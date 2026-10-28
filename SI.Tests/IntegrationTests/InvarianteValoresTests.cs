using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;
using SI.Web.Services;
using Xunit.Sdk;

namespace SI.Tests.IntegrationTests;

/// <summary>
/// Pruebas de integración para verificar el invariante fundamental de integridad patrimonial (H-06, SI-028, CP-19, RNF-01, RF-05, RB-08):
/// Tras cada movimiento de inventario, los valores del producto deben coincidir exactamente con el historial de movimientos y con las capas PEPS.
/// </summary>
[Collection("Database")]
public class InvarianteValoresTests(TestDbContextFixture fixture)
{
    /// <summary>
    /// Verifica que se cumplan estrictamente las 3 igualdades fundamentales del invariante:
    /// 1. Valor PEPS: Producto.ValorInventarioPeps == Σ(capas disponibles × costo) == Movimiento.SaldoValorPeps
    /// 2. Valor Promedio: Producto.ValorInventarioPromedio == Movimiento.SaldoValorPromedio
    /// 3. Existencia: Producto.ExistenciaActual == Σ entradas − Σ salidas == Movimiento.ExistenciaResultante
    /// </summary>
    private static async Task VerificarInvarianteAsync(int productoId, InventarioDbContext context)
    {
        var producto = await context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productoId);
        Assert.NotNull(producto);

        var capas = await context.CapasInventarioPeps
            .AsNoTracking()
            .Where(c => c.ProductoId == productoId && c.CantidadDisponible > 0)
            .ToListAsync();

        var sumaCapasPeps = capas.Sum(c => PoliticaRedondeo.Importe(c.CantidadDisponible * c.CostoUnitario));

        var movimientos = await context.Movimientos
            .AsNoTracking()
            .Where(m => m.ProductoId == productoId)
            .OrderBy(m => m.Fecha)
            .ThenBy(m => m.Id)
            .ToListAsync();

        Assert.NotEmpty(movimientos);
        var ultimoMovimiento = movimientos[^1];

        var sumaEntradas = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Entrada)
            .Sum(m => m.Cantidad);

        var sumaSalidas = movimientos
            .Where(m => m.Tipo == TipoMovimiento.Salida)
            .Sum(m => m.Cantidad);

        var existenciaCalculada = sumaEntradas - sumaSalidas;

        // 1. Invariante PEPS
        Assert.Equal(sumaCapasPeps, producto.ValorInventarioPeps);
        Assert.Equal(ultimoMovimiento.SaldoValorPeps, producto.ValorInventarioPeps);

        // 2. Invariante Promedio
        Assert.Equal(ultimoMovimiento.SaldoValorPromedio, producto.ValorInventarioPromedio);

        // 3. Invariante Existencia
        Assert.Equal(existenciaCalculada, producto.ExistenciaActual);
        Assert.Equal(ultimoMovimiento.ExistenciaResultante, producto.ExistenciaActual);
    }

    [Fact]
    [Trait("CP", "CP-19")]
    [Trait("RNF", "RNF-01")]
    [Trait("RF", "RF-05")]
    [Trait("RB", "RB-08")]
    [Trait("Nivel", "Integracion")]
    public async Task Invariante_SeCumpleTrasCadaMovimiento_EscenarioE01()
    {
        // Arrange — Limpieza y creación de producto P-001
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
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
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;
        }

        // Movimiento 1: E1 (10 @ 20.00)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 9, 0, 0), 10.0000m, 20.0000m);
            Assert.True(res.Exitoso);
        }

        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }

        // Movimiento 2: E2 (10 @ 30.00)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 9, 0, 0), 10.0000m, 30.0000m);
            Assert.True(res.Exitoso);
        }

        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }

        // Movimiento 3: S1 (Salida de 5 unidades)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m);
            Assert.True(res.Exitoso);
        }

        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }
    }

    [Fact]
    [Trait("CP", "CP-19")]
    [Trait("RNF", "RNF-01")]
    [Trait("RF", "RF-05")]
    [Trait("RB", "RB-08")]
    [Trait("Nivel", "Integracion")]
    public async Task Invariante_SeCumpleTrasCadaMovimiento_EscenarioE02()
    {
        // Arrange — Limpieza y creación de producto P-002
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-002",
                Nombre = "Arandela 1/4",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;
        }

        // Movimiento 1: E1 (3 @ 10.00)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 8, 0, 0), 3.0000m, 10.0000m);
            Assert.True(res.Exitoso);
        }
        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }

        // Movimiento 2: E2 (4 @ 11.00)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 8, 0, 0), 4.0000m, 11.0000m);
            Assert.True(res.Exitoso);
        }
        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }

        // Movimiento 3: S1 (5 unidades multicapa)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 8, 0, 0), 5.0000m);
            Assert.True(res.Exitoso);
        }
        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }

        // Movimiento 4: S2 (2 unidades, agota existencia a cero con ajuste D-03)
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            var res = await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 4, 8, 0, 0), 2.0000m);
            Assert.True(res.Exitoso);
        }
        await using (var context = fixture.CreateDbContext())
        {
            await VerificarInvarianteAsync(productoId, context);
        }
    }

    [Fact]
    [Trait("CP", "CP-19")]
    [Trait("RNF", "RNF-01")]
    [Trait("RF", "RF-05")]
    [Trait("RB", "RB-08")]
    [Trait("Nivel", "Integracion")]
    public async Task Invariante_FallaSiSeAlteraCualquieraDeLasTresIgualdades()
    {
        // Arrange — Configurar un producto con estado válido
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-003",
                Nombre = "Clavo 2 pulg",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;

            IInventarioService service = new InventarioService(contextSetup);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 9, 0, 0), 10.0000m, 20.0000m);
        }

        // Caso 1: Se rompe la igualdad PEPS (discrepancia artificial entre producto y capas)
        await using (var contextCorrupcionPeps = fixture.CreateDbContext())
        {
            var prod = await contextCorrupcionPeps.Productos.FirstAsync(p => p.Id == productoId);
            prod.ValorInventarioPeps += 0.05m;
            await contextCorrupcionPeps.SaveChangesAsync();

            await Assert.ThrowsAnyAsync<XunitException>(() =>
                VerificarInvarianteAsync(productoId, contextCorrupcionPeps));

            // Restaurar
            prod.ValorInventarioPeps -= 0.05m;
            await contextCorrupcionPeps.SaveChangesAsync();
        }

        // Caso 2: Se rompe la igualdad Promedio (discrepancia artificial con el último movimiento)
        await using (var contextCorrupcionProm = fixture.CreateDbContext())
        {
            var prod = await contextCorrupcionProm.Productos.FirstAsync(p => p.Id == productoId);
            prod.ValorInventarioPromedio += 10.00m;
            await contextCorrupcionProm.SaveChangesAsync();

            await Assert.ThrowsAnyAsync<XunitException>(() =>
                VerificarInvarianteAsync(productoId, contextCorrupcionProm));

            // Restaurar
            prod.ValorInventarioPromedio -= 10.00m;
            await contextCorrupcionProm.SaveChangesAsync();
        }

        // Caso 3: Se rompe la igualdad de Existencia (discrepancia artificial con historial)
        await using (var contextCorrupcionStock = fixture.CreateDbContext())
        {
            var prod = await contextCorrupcionStock.Productos.FirstAsync(p => p.Id == productoId);
            prod.ExistenciaActual += 1.0000m;
            await contextCorrupcionStock.SaveChangesAsync();

            await Assert.ThrowsAnyAsync<XunitException>(() =>
                VerificarInvarianteAsync(productoId, contextCorrupcionStock));

            // Restaurar
            prod.ExistenciaActual -= 1.0000m;
            await contextCorrupcionStock.SaveChangesAsync();
        }
    }
}
