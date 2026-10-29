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
public class ConsultaExistenciasPageTests(TestDbContextFixture fixture)
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
    public async Task ConsultaModel_OnGetAsync_SinProductoSeleccionado_CargaListaYResultadoNulo()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var ctx = fixture.CreateDbContext();
        ctx.Productos.Add(new Producto
        {
            Codigo = "PRD-A",
            Nombre = "Tornillo A",
            UnidadMedida = "pieza",
            Activo = true
        });
        await ctx.SaveChangesAsync();

        var inventarioService = new InventarioService(ctx);
        var productoService = new ProductoService(ctx);
        var page = new ConsultaModel(inventarioService, productoService);
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.Single(page.ProductosDisponibles);
        Assert.Null(page.Resultado);
        Assert.Null(page.ResultadoComparativo);
        Assert.Null(page.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_ProductoInexistente_EstableceMensajeError()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var ctx = fixture.CreateDbContext();
        var inventarioService = new InventarioService(ctx);
        var productoService = new ProductoService(ctx);
        var page = new ConsultaModel(inventarioService, productoService)
        {
            ProductoId = 8888,
            Metodo = MetodoCosteo.Peps
        };
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.NotNull(page.MensajeError);
        Assert.Contains("El producto no existe", page.MensajeError, StringComparison.OrdinalIgnoreCase);
        Assert.Null(page.ResultadoComparativo);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("D", "D-09")]
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_ConProductoYMovimientos_CargaResultadoYComparativo()
    {
        // Arrange — Preparar Escenario E-01 (10 @ 20, 10 @ 30, salida 5)
        fixture.ResetDatabase();
        int prodId;
        await using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "PRD-E01",
                Nombre = "Clavo Galvanizado",
                UnidadMedida = "caja",
                Activo = true
            };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            prodId = p.Id;

            var inv = new InventarioService(ctx);
            await inv.RegistrarEntradaAsync(prodId, DateTime.UtcNow.AddDays(-2), 10m, 20m);
            await inv.RegistrarEntradaAsync(prodId, DateTime.UtcNow.AddDays(-1), 10m, 30m);
            await inv.RegistrarSalidaAsync(prodId, DateTime.UtcNow, 5m);
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

        // Assert
        Assert.NotNull(page.Resultado);
        Assert.True(page.Resultado.Encontrado);
        Assert.Equal(15.0000m, page.Resultado.ExistenciaActual);
        Assert.Equal(400.0000m, page.Resultado.ValorInventario);
        Assert.Equal(26.6667m, page.Resultado.CostoUnitario);
        Assert.Equal(2, page.Resultado.CapasActivas.Count);

        // Comparativa cargada automáticamente
        Assert.NotNull(page.ResultadoComparativo);
        Assert.Equal(MetodoCosteo.PromedioPonderado, page.ResultadoComparativo.Metodo);
        Assert.Equal(15.0000m, page.ResultadoComparativo.ExistenciaActual);
        Assert.Equal(375.0000m, page.ResultadoComparativo.ValorInventario);
        Assert.Equal(25.0000m, page.ResultadoComparativo.CostoUnitario);
    }
}
