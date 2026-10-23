using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Pages.Productos;
using SI.Web.Services;

namespace SI.Tests.FunctionalTests;

[Collection("Database")]
public class ProductosPagesTests(TestDbContextFixture fixture)
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
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task IndexModel_OnGetAsync_CargaListadoCompletoConEstadoYExistencia()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            await service.RegistrarAsync("P-001", "Tornillo 1/4", "pieza");
            await service.RegistrarAsync("P-002", "Tuerca M6", "pieza");
        }

        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);
        var page = new IndexModel(serviceLectura);
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.NotNull(page.Productos);
        Assert.Equal(2, page.Productos.Count);
        Assert.Contains(page.Productos, p => p.Codigo == "P-001" && p.Activo && p.ExistenciaActual == 0m);
        Assert.Contains(page.Productos, p => p.Codigo == "P-002" && p.Activo && p.ExistenciaActual == 0m);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RB", "RB-10")]
    [Trait("Nivel", "Funcional")]
    public async Task IndexModel_OnPostCambiarEstadoAsync_AlternaEstadoYRedirige()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            var res = await service.RegistrarAsync("P-001", "Tornillo", "pieza");
            productoId = res.EntidadId!.Value;
        }

        using var contextPost = fixture.CreateDbContext();
        var servicePost = new ProductoService(contextPost);
        var page = new IndexModel(servicePost);
        ConfigurarContextoPagina(page);

        // Act: Desactivar producto
        var resultado = await page.OnPostCambiarEstadoAsync(productoId, nuevoEstado: false);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(resultado);
        Assert.Equal("./Index", redirect.PageName);

        using var contextVerif = fixture.CreateDbContext();
        var producto = await contextVerif.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
        Assert.NotNull(producto);
        Assert.False(producto.Activo);
    }

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-02")]
    [Trait("Nivel", "Funcional")]
    public async Task CrearModel_OnPostAsync_ProductoValido_RegistraYRedirigeAIndex()
    {
        // Arrange
        fixture.ResetDatabase();
        using var context = fixture.CreateDbContext();
        var service = new ProductoService(context);
        var page = new CrearModel(service);
        ConfigurarContextoPagina(page);

        page.Input = new CrearModel.InputModel
        {
            Codigo = "P-100",
            Nombre = "Arandela 5/16",
            UnidadMedida = "pieza",
            Descripcion = "Acero templado"
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(resultado);
        Assert.Equal("./Index", redirect.PageName);

        using var contextVerif = fixture.CreateDbContext();
        var guardado = await contextVerif.Productos.FirstOrDefaultAsync(p => p.Codigo == "P-100");
        Assert.NotNull(guardado);
        Assert.Equal("Arandela 5/16", guardado.Nombre);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task CrearModel_OnPostAsync_CodigoDuplicado_AgregaErrorDeDominioYPermaneceEnPagina()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            await service.RegistrarAsync("P-001", "Original", "pieza");
        }

        using var context = fixture.CreateDbContext();
        var servicePost = new ProductoService(context);
        var page = new CrearModel(servicePost);
        ConfigurarContextoPagina(page);

        page.Input = new CrearModel.InputModel
        {
            Codigo = "P-001",
            Nombre = "Duplicado",
            UnidadMedida = "pieza"
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert: Permanece en la página (PageResult) y el ModelState tiene el error exacto del backend
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage.Contains("Ya existe un producto con el código P-001"));
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task CrearModel_OnPostAsync_DatosInvalidos_AgregaErrorDeValidacionYPermaneceEnPagina()
    {
        // Arrange
        fixture.ResetDatabase();
        using var context = fixture.CreateDbContext();
        var service = new ProductoService(context);
        var page = new CrearModel(service);
        ConfigurarContextoPagina(page);

        page.Input = new CrearModel.InputModel
        {
            Codigo = "",
            Nombre = "",
            UnidadMedida = ""
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == ValidadorProducto.MensajeCodigoObligatorio);
    }

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Funcional")]
    public async Task EditarModel_OnGetAsync_ProductoExistente_CargaDatosEnFormulario()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            var res = await service.RegistrarAsync("P-050", "Remache", "pieza", "Aluminio");
            productoId = res.EntidadId!.Value;
        }

        using var context = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(context);
        var page = new EditarModel(serviceLectura);
        ConfigurarContextoPagina(page);

        // Act
        var resultado = await page.OnGetAsync(productoId);

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.Equal("P-050", page.Input.Codigo);
        Assert.Equal("Remache", page.Input.Nombre);
        Assert.Equal("pieza", page.Input.UnidadMedida);
        Assert.Equal("Aluminio", page.Input.Descripcion);
        Assert.True(page.Input.Activo);
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-01")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Funcional")]
    public async Task EditarModel_OnGetAsync_ProductoInexistente_RedirigeAIndexConMensajeError()
    {
        // Arrange
        fixture.ResetDatabase();
        using var context = fixture.CreateDbContext();
        var service = new ProductoService(context);
        var page = new EditarModel(service);
        ConfigurarContextoPagina(page);

        // Act
        var resultado = await page.OnGetAsync(99999);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(resultado);
        Assert.Equal("./Index", redirect.PageName);
        Assert.Equal(ProductoService.MensajeProductoNoExiste, page.TempData["MensajeError"]);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RB", "RB-10")]
    [Trait("Nivel", "Funcional")]
    public async Task EditarModel_OnPostAsync_DatosValidos_ModificaProductoYRedirige()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            var res = await service.RegistrarAsync("P-060", "Tuerca", "pieza");
            productoId = res.EntidadId!.Value;
        }

        using var context = fixture.CreateDbContext();
        var servicePost = new ProductoService(context);
        var page = new EditarModel(servicePost);
        ConfigurarContextoPagina(page);

        page.Input = new EditarModel.InputModel
        {
            Id = productoId,
            Codigo = "P-060",
            Nombre = "Tuerca Hexagonal",
            UnidadMedida = "caja",
            Descripcion = "Zincada",
            Activo = false
        };

        // Act
        var resultado = await page.OnPostAsync(productoId);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(resultado);
        Assert.Equal("./Index", redirect.PageName);

        using var contextVerif = fixture.CreateDbContext();
        var actualizado = await contextVerif.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
        Assert.NotNull(actualizado);
        Assert.Equal("P-060", actualizado.Codigo); // Inmutable D-10
        Assert.Equal("Tuerca Hexagonal", actualizado.Nombre);
        Assert.Equal("caja", actualizado.UnidadMedida);
        Assert.Equal("Zincada", actualizado.Descripcion);
        Assert.False(actualizado.Activo);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task EditarModel_OnPostAsync_DatosInvalidos_AgregaErrorYPermaneceEnPagina()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            var res = await service.RegistrarAsync("P-070", "Cable", "metro");
            productoId = res.EntidadId!.Value;
        }

        using var context = fixture.CreateDbContext();
        var servicePost = new ProductoService(context);
        var page = new EditarModel(servicePost);
        ConfigurarContextoPagina(page);

        page.Input = new EditarModel.InputModel
        {
            Id = productoId,
            Codigo = "P-070",
            Nombre = "",
            UnidadMedida = "metro"
        };

        // Act
        var resultado = await page.OnPostAsync(productoId);

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == ValidadorProducto.MensajeNombreObligatorio);
    }
}
