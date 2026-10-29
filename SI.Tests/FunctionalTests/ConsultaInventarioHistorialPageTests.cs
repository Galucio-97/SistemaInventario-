using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Pages.Inventario;
using SI.Web.Services;

namespace SI.Tests.FunctionalTests;

[Collection("Database")]
public class ConsultaInventarioHistorialPageTests(TestDbContextFixture fixture)
{
    private class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext? context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext? context, IDictionary<string, object>? values) { }
    }

    private static void ConfigurarContextoPagina(PageModel page)
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var tempData = new TempDataDictionary(httpContext, new FakeTempDataProvider());

        page.PageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };
        page.TempData = tempData;
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_ConProductoSeleccionado_CargaHistorialMovimientosOrdenado()
    {
        // Arrange
        fixture.ResetDatabase();
        int prodId;
        await using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "PRD-HIST",
                Nombre = "Producto Para Historial",
                UnidadMedida = "pieza",
                Activo = true
            };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            prodId = p.Id;

            var inv = new InventarioService(ctx);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 1, 10, 0, 0), 10m, 20m);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 2, 10, 0, 0), 10m, 30m);
            await inv.RegistrarSalidaAsync(prodId, new DateTime(2026, 10, 3, 10, 0, 0), 5m);
        }

        await using var ctxConsulta = fixture.CreateDbContext();
        var inventarioService = new InventarioService(ctxConsulta);
        var productoService = new ProductoService(ctxConsulta);
        var page = new ConsultaModel(inventarioService, productoService)
        {
            ProductoId = prodId,
            Metodo = MetodoCosteo.PromedioPonderado
        };
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert — Debe cargar el Historial
        Assert.NotNull(page.Historial);
        Assert.True(page.Historial.Encontrado);
        Assert.Equal(3, page.Historial.Movimientos.Count);
        Assert.Equal(MetodoCosteo.PromedioPonderado, page.Historial.Metodo);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_MetodoPeps_CargaHistorialConCostosPepsYCapasActivas()
    {
        // Arrange — Escenario E-01
        fixture.ResetDatabase();
        int prodId;
        await using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "PRD-E01-PAGE",
                Nombre = "Producto E-01 Page",
                UnidadMedida = "pieza",
                Activo = true
            };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            prodId = p.Id;

            var inv = new InventarioService(ctx);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 1, 10, 0, 0), 10m, 20m);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 2, 10, 0, 0), 10m, 30m);
            await inv.RegistrarSalidaAsync(prodId, new DateTime(2026, 10, 3, 10, 0, 0), 5m);
        }

        await using var ctxConsulta = fixture.CreateDbContext();
        var inventarioService = new InventarioService(ctxConsulta);
        var productoService = new ProductoService(ctxConsulta);
        var page = new ConsultaModel(inventarioService, productoService)
        {
            ProductoId = prodId,
            Metodo = MetodoCosteo.Peps
        };
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert — Historial en PEPS con oráculo E-01
        Assert.NotNull(page.Historial);
        Assert.Equal(3, page.Historial.Movimientos.Count);

        var salida = page.Historial.Movimientos[2];
        Assert.Equal(TipoMovimiento.Salida, salida.Tipo);
        Assert.Equal(5.0000m, salida.Cantidad);
        Assert.Equal(20.0000m, salida.CostoUnitario);
        Assert.Equal(100.0000m, salida.CostoTotal);
        Assert.Equal(15.0000m, salida.ExistenciaResultante);
        Assert.Equal(400.0000m, salida.SaldoValor);

        // Capas activas PEPS presentes
        Assert.NotNull(page.Resultado);
        Assert.Equal(2, page.Resultado.CapasActivas.Count);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_MetodoPromedio_CargaHistorialConCostosPromedio()
    {
        // Arrange — Escenario E-01
        fixture.ResetDatabase();
        int prodId;
        await using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "PRD-E01-PROM",
                Nombre = "Producto E-01 Promedio",
                UnidadMedida = "pieza",
                Activo = true
            };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            prodId = p.Id;

            var inv = new InventarioService(ctx);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 1, 10, 0, 0), 10m, 20m);
            await inv.RegistrarEntradaAsync(prodId, new DateTime(2026, 10, 2, 10, 0, 0), 10m, 30m);
            await inv.RegistrarSalidaAsync(prodId, new DateTime(2026, 10, 3, 10, 0, 0), 5m);
        }

        await using var ctxConsulta = fixture.CreateDbContext();
        var inventarioService = new InventarioService(ctxConsulta);
        var productoService = new ProductoService(ctxConsulta);
        var page = new ConsultaModel(inventarioService, productoService)
        {
            ProductoId = prodId,
            Metodo = MetodoCosteo.PromedioPonderado
        };
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert — Historial en Promedio Ponderado con oráculo E-01
        Assert.NotNull(page.Historial);
        Assert.Equal(3, page.Historial.Movimientos.Count);

        var salida = page.Historial.Movimientos[2];
        Assert.Equal(TipoMovimiento.Salida, salida.Tipo);
        Assert.Equal(5.0000m, salida.Cantidad);
        Assert.Equal(25.0000m, salida.CostoUnitario);
        Assert.Equal(125.0000m, salida.CostoTotal);
        Assert.Equal(15.0000m, salida.ExistenciaResultante);
        Assert.Equal(375.0000m, salida.SaldoValor);
    }
}
