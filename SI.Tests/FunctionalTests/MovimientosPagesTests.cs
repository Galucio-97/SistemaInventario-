using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Data.Entities;
using SI.Web.Pages.Movimientos;
using SI.Web.Services;

namespace SI.Tests.FunctionalTests;

[Collection("Database")]
public class MovimientosPagesTests(TestDbContextFixture fixture)
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
    [Trait("CP", "CP-18")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-10")]
    [Trait("Nivel", "Funcional")]
    public async Task EntradaModel_OnGetAsync_CargaSoloProductosActivos()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            ctx.Productos.Add(new Producto { Codigo = "P-001", Nombre = "Activo 1", UnidadMedida = "pza", Activo = true });
            ctx.Productos.Add(new Producto { Codigo = "P-002", Nombre = "Inactivo 2", UnidadMedida = "pza", Activo = false });
            ctx.Productos.Add(new Producto { Codigo = "P-003", Nombre = "Activo 3", UnidadMedida = "pza", Activo = true });
            await ctx.SaveChangesAsync();
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new EntradaModel(context, service);
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.NotNull(page.ProductosActivos);
        Assert.Equal(2, page.ProductosActivos.Count);
        Assert.All(page.ProductosActivos, p => Assert.True(p.Activo));
        Assert.Contains(page.ProductosActivos, p => p.Codigo == "P-001");
        Assert.Contains(page.ProductosActivos, p => p.Codigo == "P-003");
        Assert.DoesNotContain(page.ProductosActivos, p => p.Codigo == "P-002");
    }

    [Fact]
    [Trait("CP", "CP-03")]
    [Trait("RF", "RF-02")]
    [Trait("RNF", "RNF-02")]
    [Trait("Nivel", "Funcional")]
    public async Task EntradaModel_OnPostAsync_EntradaValida_RegistraYCalculaNuevaExistencia()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto { Codigo = "P-001", Nombre = "Tornillo", UnidadMedida = "pza", Activo = true, ExistenciaActual = 0m };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new EntradaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new EntradaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 20),
            Cantidad = 10m,
            CostoUnitario = 20.00m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.True(page.ModelState.IsValid);
        Assert.NotNull(page.Resultado);
        Assert.Equal(10m, page.Resultado.ExistenciaNueva);
        Assert.Equal(200.00m, page.Resultado.CostoTotalPeps);

        using var verif = fixture.CreateDbContext();
        var guardado = await verif.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
        Assert.NotNull(guardado);
        Assert.Equal(10m, guardado.ExistenciaActual);
        Assert.Equal(20.0000m, guardado.CostoPromedioActual);
    }

    [Fact]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task EntradaModel_OnPostAsync_CantidadMenorOIgualACero_RechazaYConservaDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto { Codigo = "P-001", Nombre = "Tornillo", UnidadMedida = "pza", Activo = true };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new EntradaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new EntradaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 20),
            Cantidad = 0m,
            CostoUnitario = 15.00m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert: Permanece en la página, ModelState tiene error y datos se conservan
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == "La cantidad debe ser mayor que cero.");
        Assert.Equal(0m, page.Input.Cantidad);
        Assert.Equal(15.00m, page.Input.CostoUnitario);
        Assert.Null(page.Resultado);
    }

    [Fact]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-02")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task EntradaModel_OnPostAsync_CostoMenorOIgualACero_RechazaYConservaDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto { Codigo = "P-001", Nombre = "Tornillo", UnidadMedida = "pza", Activo = true };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new EntradaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new EntradaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 20),
            Cantidad = 5m,
            CostoUnitario = -1.00m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == "El costo unitario debe ser mayor que cero.");
        Assert.Equal(5m, page.Input.Cantidad);
        Assert.Equal(-1.00m, page.Input.CostoUnitario);
    }

    [Fact]
    [Trait("CP", "CP-18")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-10")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task EntradaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto { Codigo = "P-002", Nombre = "Inactivo", UnidadMedida = "pza", Activo = false };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new EntradaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new EntradaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 20),
            Cantidad = 5m,
            CostoUnitario = 10.00m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == InventarioService.MensajeProductoInactivo);
    }

    [Fact]
    [Trait("RF", "RF-03")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Funcional")]
    public async Task SalidaModel_OnGetAsync_CargaProductosConExistencia()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            ctx.Productos.Add(new Producto { Codigo = "P-001", Nombre = "Con Existencia", UnidadMedida = "pza", Activo = true, ExistenciaActual = 10m });
            ctx.Productos.Add(new Producto { Codigo = "P-002", Nombre = "Sin Existencia", UnidadMedida = "pza", Activo = true, ExistenciaActual = 0m });
            await ctx.SaveChangesAsync();
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new SalidaModel(context, service);
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.NotNull(page.ProductosActivos);
        Assert.Equal(2, page.ProductosActivos.Count);
        Assert.Contains(page.ProductosActivos, p => p.Codigo == "P-001" && p.ExistenciaActual == 10m);
    }

    [Fact]
    [Trait("CP", "CP-04")]
    [Trait("RF", "RF-03")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Funcional")]
    public async Task SalidaModel_OnPostAsync_SalidaValida_RegistraYMuestraCostosPorAmbosMetodos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var serviceInicial = new InventarioService(ctx);
            var p = new Producto { Codigo = "P-001", Nombre = "Tornillo", UnidadMedida = "pza", Activo = true };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;

            // Entrada inicial 10 @ 20.00
            await serviceInicial.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 20), 10m, 20.00m);
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new SalidaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new SalidaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 21),
            Cantidad = 4m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert: muestra la nueva existencia (6) y los costos calculados
        Assert.IsType<PageResult>(resultado);
        Assert.True(page.ModelState.IsValid);
        Assert.NotNull(page.Resultado);
        Assert.Equal(6m, page.Resultado.ExistenciaNueva);
        Assert.Equal(80.00m, page.Resultado.CostoTotalPeps);
        Assert.Equal(20.0000m, page.Resultado.CostoUnitarioPeps);
        Assert.Equal(80.00m, page.Resultado.CostoTotalPromedio);
        Assert.Equal(20.0000m, page.Resultado.CostoUnitarioPromedio);
    }

    [Fact]
    [Trait("CP", "CP-07")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-03")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task SalidaModel_OnPostAsync_CantidadExcedeExistencia_RechazaYConservaDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var serviceInicial = new InventarioService(ctx);
            var p = new Producto { Codigo = "P-001", Nombre = "Tornillo", UnidadMedida = "pza", Activo = true };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;

            // Entrada 5 @ 10.00
            await serviceInicial.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 20), 5m, 10.00m);
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new SalidaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new SalidaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 21),
            Cantidad = 10m // Excede 5
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert: Rechazado, conserva 10m y no modifica
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage.Contains("Existencia insuficiente"));
        Assert.Equal(10m, page.Input.Cantidad);
        Assert.Null(page.Resultado);
    }

    [Fact]
    [Trait("CP", "CP-18")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-10")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Funcional")]
    public async Task SalidaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto { Codigo = "P-002", Nombre = "Inactivo", UnidadMedida = "pza", Activo = false, ExistenciaActual = 10m };
            ctx.Productos.Add(p);
            await ctx.SaveChangesAsync();
            productoId = p.Id;
        }

        using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        var page = new SalidaModel(context, service);
        ConfigurarContextoPagina(page);

        page.Input = new SalidaModel.InputModel
        {
            ProductoId = productoId,
            Fecha = new DateTime(2026, 10, 21),
            Cantidad = 2m
        };

        // Act
        var resultado = await page.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(resultado);
        Assert.False(page.ModelState.IsValid);
        Assert.Contains(page.ModelState.Values.SelectMany(v => v.Errors),
            e => e.ErrorMessage == InventarioService.MensajeProductoInactivo);
    }
}
