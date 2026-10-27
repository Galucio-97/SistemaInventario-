using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

/// <summary>
/// Pruebas de integración para verificar la atomicidad y el rollback automático ante fallos
/// de infraestructura antes de confirmar transacciones (RNF-04, RB-06, CP-13, SI-026).
/// </summary>
[Collection("Database")]
public class TransaccionRollbackTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-13")]
    [Trait("RNF", "RNF-04")]
    [Trait("RB", "RB-06")]
    [Trait("Nivel", "Integracion")]
    public async Task SalidaConFalloEnCommit_HaceRollbackCompletoYNoDejaActualizacionesParciales()
    {
        // Arrange — Configurar estado E1 + E2 de E-01 (CP-13)
        // 20 unidades, 500.00 promedio / 500.00 PEPS, capas [10 @ 20.00, 10 @ 30.00], 2 movimientos
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            contextSetup.Productos.Add(producto);
            await contextSetup.SaveChangesAsync();
            productoId = producto.Id;

            IInventarioService inventarioServiceSetup = new InventarioService(contextSetup);

            // E1: 10 @ 20.00
            var resE1 = await inventarioServiceSetup.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 1, 9, 0, 0), 10.0000m, 20.0000m);
            Assert.True(resE1.Exitoso);

            // E2: 10 @ 30.00
            var resE2 = await inventarioServiceSetup.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 2, 9, 0, 0), 10.0000m, 30.0000m);
            Assert.True(resE2.Exitoso);
        }

        // Interceptor configurado para lanzar excepción al momento de invocar TransactionCommittingAsync
        var interceptorFallo = new SimuladorFalloTransaccionInterceptor { FalloActivado = true };

        // Act — Intentar registrar salida S1 (5 unidades) en un contexto con el interceptor de fallo
        await using (var contextOperacion = fixture.CreateDbContext(interceptorFallo))
        {
            IInventarioService serviceConFallo = new InventarioService(contextOperacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                serviceConFallo.RegistrarSalidaAsync(
                    productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m));

            Assert.Equal(SimuladorFalloTransaccionInterceptor.MensajeFalloSimulado, ex.Message);
        }

        // Assert — Verificar con un contexto desacoplado y nuevo que la BD está exactamente en estado E1 + E2 (CP-13)
        await using (var contextVerificacion = fixture.CreateDbContext())
        {
            var producto = await contextVerificacion.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
            Assert.NotNull(producto);

            // Existencia y saldos intactos
            Assert.Equal(20.0000m, producto.ExistenciaActual);
            Assert.Equal(25.0000m, producto.CostoPromedioActual);
            Assert.Equal(500.00m, producto.ValorInventarioPromedio);
            Assert.Equal(500.00m, producto.ValorInventarioPeps);

            // Capas PEPS intactas: exactamente 2 capas completas (10 y 10 disponibles)
            var capas = await contextVerificacion.CapasInventarioPeps
                .Where(c => c.ProductoId == productoId)
                .OrderBy(c => c.FechaEntrada)
                .ToListAsync();

            Assert.Equal(2, capas.Count);
            Assert.Equal(10.0000m, capas[0].CantidadDisponible);
            Assert.Equal(10.0000m, capas[0].CantidadInicial);
            Assert.Equal(20.0000m, capas[0].CostoUnitario);

            Assert.Equal(10.0000m, capas[1].CantidadDisponible);
            Assert.Equal(10.0000m, capas[1].CantidadInicial);
            Assert.Equal(30.0000m, capas[1].CostoUnitario);

            // Exactamente 2 movimientos (las entradas E1 y E2, ninguna salida registrada)
            var movimientos = await contextVerificacion.Movimientos
                .Where(m => m.ProductoId == productoId)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            Assert.Equal(2, movimientos.Count);
            Assert.All(movimientos, m => Assert.Equal(TipoMovimiento.Entrada, m.Tipo));
        }
    }

    [Fact]
    [Trait("CP", "CP-13")]
    [Trait("RNF", "RNF-04")]
    [Trait("RB", "RB-06")]
    [Trait("Nivel", "Integracion")]
    public async Task EntradaConFalloEnCommit_HaceRollbackCompletoYNoDejaActualizacionesParciales()
    {
        // Arrange — Producto inicial sin movimientos
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "P-002",
                Nombre = "Tuerca 1/4",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            contextSetup.Productos.Add(producto);
            await contextSetup.SaveChangesAsync();
            productoId = producto.Id;
        }

        var interceptorFallo = new SimuladorFalloTransaccionInterceptor { FalloActivado = true };

        // Act — Intentar registrar entrada en un contexto con el interceptor de fallo activo
        await using (var contextOperacion = fixture.CreateDbContext(interceptorFallo))
        {
            IInventarioService serviceConFallo = new InventarioService(contextOperacion);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                serviceConFallo.RegistrarEntradaAsync(
                    productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m));

            Assert.Equal(SimuladorFalloTransaccionInterceptor.MensajeFalloSimulado, ex.Message);
        }

        // Assert — Verificar con contexto desacoplado y nuevo que no se guardó ningún cambio en BD
        await using (var contextVerificacion = fixture.CreateDbContext())
        {
            var producto = await contextVerificacion.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
            Assert.NotNull(producto);

            // Existencia y saldos intactos en cero
            Assert.Equal(0.0000m, producto.ExistenciaActual);
            Assert.Equal(0.0000m, producto.CostoPromedioActual);
            Assert.Equal(0.00m, producto.ValorInventarioPromedio);
            Assert.Equal(0.00m, producto.ValorInventarioPeps);

            // Cero capas PEPS creadas
            var capas = await contextVerificacion.CapasInventarioPeps
                .Where(c => c.ProductoId == productoId)
                .ToListAsync();
            Assert.Empty(capas);

            // Cero movimientos registrados
            var movimientos = await contextVerificacion.Movimientos
                .Where(m => m.ProductoId == productoId)
                .ToListAsync();
            Assert.Empty(movimientos);
        }
    }
}
