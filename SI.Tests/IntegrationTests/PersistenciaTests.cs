using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

/// <summary>
/// Pruebas de integración para verificar la persistencia y durabilidad de datos tras reinicio
/// o ciclo de vida independiente de la aplicación (RNF-02, RB-05, CP-12, SI-027).
/// </summary>
[Collection("Database")]
public class PersistenciaTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-12")]
    [Trait("RNF", "RNF-02")]
    [Trait("RB", "RB-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ReinicioAplicacion_ConservaDatos_EscenarioE01Completo()
    {
        // Arrange — Limpieza y configuración inicial
        fixture.ResetDatabase();
        int productoId;

        // Sesión 1: Creación del producto P-001
        await using (var contextSesion1 = fixture.CreateDbContext())
        {
            var p = new Producto
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
            contextSesion1.Productos.Add(p);
            await contextSesion1.SaveChangesAsync();
            productoId = p.Id;
        } // contextSesion1 desechado y cerrado

        // Sesión 2: Registrar Movimiento E1 (10 @ 20.00)
        await using (var contextSesion2 = fixture.CreateDbContext())
        {
            IInventarioService inventarioService = new InventarioService(contextSesion2);
            var resE1 = await inventarioService.RegistrarEntradaAsync(
                productoId,
                new DateTime(2026, 10, 1, 9, 0, 0),
                10.0000m,
                20.0000m
            );
            Assert.True(resE1.Exitoso);
        } // contextSesion2 desechado y cerrado

        // Sesión 3: Registrar Movimiento E2 (10 @ 30.00)
        await using (var contextSesion3 = fixture.CreateDbContext())
        {
            IInventarioService inventarioService = new InventarioService(contextSesion3);
            var resE2 = await inventarioService.RegistrarEntradaAsync(
                productoId,
                new DateTime(2026, 10, 2, 9, 0, 0),
                10.0000m,
                30.0000m
            );
            Assert.True(resE2.Exitoso);
        } // contextSesion3 desechado y cerrado

        // Sesión 4: Registrar Movimiento S1 (Salida de 5 unidades)
        await using (var contextSesion4 = fixture.CreateDbContext())
        {
            IInventarioService inventarioService = new InventarioService(contextSesion4);
            var resS1 = await inventarioService.RegistrarSalidaAsync(
                productoId,
                new DateTime(2026, 10, 3, 10, 0, 0),
                5.0000m
            );
            Assert.True(resS1.Exitoso);
        } // contextSesion4 desechado y cerrado (simulando detención completa de la aplicación)

        // Act & Assert — Sesión 5 (Reinicio de la aplicación con nuevo contexto desacoplado de lectura)
        await using (var contextReinicio = fixture.CreateDbContext())
        {
            // 1. Verificar persistencia e integridad del Producto
            var producto = await contextReinicio.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
            Assert.NotNull(producto);
            Assert.Equal("P-001", producto.Codigo);
            Assert.Equal("Tornillo 1/4", producto.Nombre);
            Assert.True(producto.Activo);
            Assert.Equal(15.0000m, producto.ExistenciaActual);
            Assert.Equal(25.0000m, producto.CostoPromedioActual);
            Assert.Equal(375.00m, producto.ValorInventarioPromedio);
            Assert.Equal(400.00m, producto.ValorInventarioPeps);

            // 2. Verificar persistencia de las Capas PEPS
            var capas = await contextReinicio.CapasInventarioPeps
                .Where(c => c.ProductoId == productoId)
                .OrderBy(c => c.FechaEntrada)
                .ThenBy(c => c.Id)
                .ToListAsync();

            Assert.Equal(2, capas.Count);

            // Capa 1: Lote inicial 10 @ 20.00, disponible 5 tras S1
            Assert.Equal(new DateTime(2026, 10, 1, 9, 0, 0), capas[0].FechaEntrada);
            Assert.Equal(10.0000m, capas[0].CantidadInicial);
            Assert.Equal(5.0000m, capas[0].CantidadDisponible);
            Assert.Equal(20.0000m, capas[0].CostoUnitario);

            // Capa 2: Lote inicial 10 @ 30.00, intacto (10 disponible)
            Assert.Equal(new DateTime(2026, 10, 2, 9, 0, 0), capas[1].FechaEntrada);
            Assert.Equal(10.0000m, capas[1].CantidadInicial);
            Assert.Equal(10.0000m, capas[1].CantidadDisponible);
            Assert.Equal(30.0000m, capas[1].CostoUnitario);

            // 3. Verificar persistencia del historial de 3 Movimientos
            var movimientos = await contextReinicio.Movimientos
                .Where(m => m.ProductoId == productoId)
                .OrderBy(m => m.Fecha)
                .ThenBy(m => m.Id)
                .ToListAsync();

            Assert.Equal(3, movimientos.Count);

            // Movimiento 1 (E1)
            var m1 = movimientos[0];
            Assert.Equal(TipoMovimiento.Entrada, m1.Tipo);
            Assert.Equal(new DateTime(2026, 10, 1, 9, 0, 0), m1.Fecha);
            Assert.Equal(10.0000m, m1.Cantidad);
            Assert.Equal(20.0000m, m1.CostoUnitarioEntrada);
            Assert.Equal(10.0000m, m1.ExistenciaResultante);
            Assert.Equal(200.00m, m1.CostoTotalPeps);
            Assert.Equal(200.00m, m1.SaldoValorPeps);
            Assert.Equal(200.00m, m1.CostoTotalPromedio);
            Assert.Equal(200.00m, m1.SaldoValorPromedio);

            // Movimiento 2 (E2)
            var m2 = movimientos[1];
            Assert.Equal(TipoMovimiento.Entrada, m2.Tipo);
            Assert.Equal(new DateTime(2026, 10, 2, 9, 0, 0), m2.Fecha);
            Assert.Equal(10.0000m, m2.Cantidad);
            Assert.Equal(30.0000m, m2.CostoUnitarioEntrada);
            Assert.Equal(20.0000m, m2.ExistenciaResultante);
            Assert.Equal(300.00m, m2.CostoTotalPeps);
            Assert.Equal(500.00m, m2.SaldoValorPeps);
            Assert.Equal(300.00m, m2.CostoTotalPromedio);
            Assert.Equal(500.00m, m2.SaldoValorPromedio);

            // Movimiento 3 (S1)
            var m3 = movimientos[2];
            Assert.Equal(TipoMovimiento.Salida, m3.Tipo);
            Assert.Equal(new DateTime(2026, 10, 3, 10, 0, 0), m3.Fecha);
            Assert.Equal(5.0000m, m3.Cantidad);
            Assert.Equal(0m, m3.CostoUnitarioEntrada);
            Assert.Equal(15.0000m, m3.ExistenciaResultante);
            Assert.Equal(20.0000m, m3.CostoUnitarioPeps);
            Assert.Equal(100.00m, m3.CostoTotalPeps);
            Assert.Equal(400.00m, m3.SaldoValorPeps);
            Assert.Equal(25.0000m, m3.CostoUnitarioPromedio);
            Assert.Equal(125.00m, m3.CostoTotalPromedio);
            Assert.Equal(375.00m, m3.SaldoValorPromedio);
        }
    }
}
