using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Enums;
using SI.Web.Interfaces;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class EjercicioReferenciaIntegrationTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task CargarEjercicioE01Async_PrimeraVez_CreaProductoYMovimientosE01ExactosConOraculo()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var context = fixture.CreateDbContext();
        var productoService = new ProductoService(context);
        var inventarioService = new InventarioService(context);
        var service = new EjercicioReferenciaService(productoService, inventarioService);

        // Act
        var resultado = await service.CargarEjercicioE01Async();

        // Assert — Creación exitosa
        Assert.True(resultado.Exitoso);
        Assert.False(resultado.YaExistia);
        Assert.True(resultado.ProductoId > 0);
        Assert.Equal(IEjercicioReferenciaService.CodigoProductoE01, resultado.Codigo);

        // Assert — Oráculo E-01 en PEPS (400.00 valor, 15 existencia, 26.6667 costo unitario per D-09)
        var peps = await inventarioService.ConsultarExistenciasAsync(resultado.ProductoId, MetodoCosteo.Peps);
        Assert.True(peps.Encontrado);
        Assert.Equal(15.0000m, peps.ExistenciaActual);
        Assert.Equal(400.0000m, peps.ValorInventario);
        Assert.Equal(26.6667m, peps.CostoUnitario);
        Assert.Equal(2, peps.CapasActivas.Count);

        // Assert — Oráculo E-01 en Promedio Ponderado (375.00 valor, 15 existencia, 25.0000 costo unitario)
        var promedio = await inventarioService.ConsultarExistenciasAsync(resultado.ProductoId, MetodoCosteo.PromedioPonderado);
        Assert.True(promedio.Encontrado);
        Assert.Equal(15.0000m, promedio.ExistenciaActual);
        Assert.Equal(375.0000m, promedio.ValorInventario);
        Assert.Equal(25.0000m, promedio.CostoUnitario);

        // Assert — Historial tipo kardex con 3 movimientos (E1, E2, S1)
        var historial = await inventarioService.ObtenerHistorialMovimientosAsync(resultado.ProductoId, MetodoCosteo.Peps);
        Assert.True(historial.Encontrado);
        Assert.Equal(3, historial.Movimientos.Count);
    }

    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task CargarEjercicioE01Async_SegundaVez_NoDuplicaProductoNiMovimientos()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var context = fixture.CreateDbContext();
        var productoService = new ProductoService(context);
        var inventarioService = new InventarioService(context);
        var service = new EjercicioReferenciaService(productoService, inventarioService);

        // Act 1 — Primera carga
        var primerResultado = await service.CargarEjercicioE01Async();
        Assert.True(primerResultado.Exitoso);
        Assert.False(primerResultado.YaExistia);

        int totalProductosEsperado = await context.Productos.CountAsync();
        int totalMovimientosEsperado = await context.Movimientos.CountAsync();
        int totalCapasEsperado = await context.CapasInventarioPeps.CountAsync();

        // Act 2 — Segunda carga (debe ser idempotente, no duplicar)
        var segundoResultado = await service.CargarEjercicioE01Async();

        // Assert
        Assert.True(segundoResultado.Exitoso);
        Assert.True(segundoResultado.YaExistia);
        Assert.Equal(primerResultado.ProductoId, segundoResultado.ProductoId);
        Assert.Equal(primerResultado.Codigo, segundoResultado.Codigo);

        // No debe haber filas adicionales en ninguna tabla
        int totalProductosActual = await context.Productos.CountAsync();
        int totalMovimientosActual = await context.Movimientos.CountAsync();
        int totalCapasActual = await context.CapasInventarioPeps.CountAsync();

        Assert.Equal(totalProductosEsperado, totalProductosActual);
        Assert.Equal(totalMovimientosEsperado, totalMovimientosActual);
        Assert.Equal(totalCapasEsperado, totalCapasActual);
    }
}
