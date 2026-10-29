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
public class VistaComparativaPageTests(TestDbContextFixture fixture)
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
    [Trait("Nivel", "Funcional")]
    public async Task ConsultaModel_OnGetAsync_CargaHistorialComparativoParalelo_ConCantidadesFisicasIdenticas()
    {
        // Arrange — Preparar Escenario E-01 (10 @ 20, 10 @ 30, salida 5)
        fixture.ResetDatabase();
        int prodId;
        await using (var ctx = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "PRD-COMP",
                Nombre = "Producto Comparativa E-01",
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
        var ejercicioService = new EjercicioReferenciaService(productoService, inventarioService);

        var page = new ConsultaModel(inventarioService, productoService, ejercicioService)
        {
            ProductoId = prodId,
            Metodo = MetodoCosteo.Peps
        };
        ConfigurarContextoPagina(page);

        // Act
        await page.OnGetAsync();

        // Assert — Criterio 1: Se muestran existencia, valor y costo de cada salida para ambos métodos en paralelo
        Assert.NotEmpty(page.HistorialComparativo);
        Assert.Equal(3, page.HistorialComparativo.Count);

        // Assert — Criterio 2: Las cantidades físicas son estrictamente idénticas en cada fila
        foreach (var fila in page.HistorialComparativo)
        {
            Assert.True(fila.Cantidad > 0);
            Assert.True(fila.ExistenciaResultante > 0);
        }

        // Fila 1: Entrada 10 @ 20.00
        var f1 = page.HistorialComparativo[0];
        Assert.Equal(TipoMovimiento.Entrada, f1.Tipo);
        Assert.Equal(10.0000m, f1.Cantidad);
        Assert.Equal(10.0000m, f1.ExistenciaResultante);
        Assert.Equal(20.0000m, f1.CostoUnitarioPeps);
        Assert.Equal(20.0000m, f1.CostoUnitarioPromedio);
        Assert.Equal(200.0000m, f1.CostoTotalPeps);
        Assert.Equal(200.0000m, f1.CostoTotalPromedio);
        Assert.Equal(200.0000m, f1.SaldoValorPeps);
        Assert.Equal(200.0000m, f1.SaldoValorPromedio);
        Assert.Equal(0.0000m, f1.DiferenciaSaldoValor);

        // Fila 2: Entrada 10 @ 30.00
        var f2 = page.HistorialComparativo[1];
        Assert.Equal(TipoMovimiento.Entrada, f2.Tipo);
        Assert.Equal(10.0000m, f2.Cantidad);
        Assert.Equal(20.0000m, f2.ExistenciaResultante);
        Assert.Equal(30.0000m, f2.CostoUnitarioPeps);
        Assert.Equal(30.0000m, f2.CostoUnitarioPromedio);
        Assert.Equal(300.0000m, f2.CostoTotalPeps);
        Assert.Equal(300.0000m, f2.CostoTotalPromedio);
        Assert.Equal(500.0000m, f2.SaldoValorPeps);
        Assert.Equal(500.0000m, f2.SaldoValorPromedio);
        Assert.Equal(0.0000m, f2.DiferenciaSaldoValor);

        // Fila 3: Salida 5 (Oráculo CP-10: PEPS = 100.00 / saldo 400.00; Promedio = 125.00 / saldo 375.00)
        var f3 = page.HistorialComparativo[2];
        Assert.Equal(TipoMovimiento.Salida, f3.Tipo);
        Assert.Equal(5.0000m, f3.Cantidad);
        Assert.Equal(15.0000m, f3.ExistenciaResultante);

        // Costo y saldo PEPS
        Assert.Equal(20.0000m, f3.CostoUnitarioPeps);
        Assert.Equal(100.0000m, f3.CostoTotalPeps);
        Assert.Equal(400.0000m, f3.SaldoValorPeps);

        // Costo y saldo Promedio
        Assert.Equal(25.0000m, f3.CostoUnitarioPromedio);
        Assert.Equal(125.0000m, f3.CostoTotalPromedio);
        Assert.Equal(375.0000m, f3.SaldoValorPromedio);

        // Diferencias patrimoniales
        Assert.Equal(-25.0000m, f3.DiferenciaCostoTotal); // En PEPS el costo fue $25 menor
        Assert.Equal(25.0000m, f3.DiferenciaSaldoValor);  // En PEPS el inventario final vale $25 más
    }
}
