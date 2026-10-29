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
using SI.Web.Enums;
using SI.Web.Pages.Inventario;
using SI.Web.Pages.Movimientos;
using SI.Web.Pages.Productos;
using SI.Web.Services;

namespace SI.Tests.FunctionalTests;

/// <summary>
/// Pruebas funcionales de flujo completo de inicio a fin (SI-036).
/// Verifican los escenarios E-01 y E-02 desde el comportamiento observable del usuario
/// (PageModels de Razor Pages) y auditan la integridad directa en base de datos SQL Server.
/// </summary>
[Collection("Database")]
public class FlujoCompletoFunctionalTests(TestDbContextFixture fixture)
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
    [Trait("CP", "CP-03")]
    [Trait("CP", "CP-04")]
    [Trait("CP", "CP-07")]
    [Trait("CP", "CP-08")]
    [Trait("CP", "CP-09")]
    [Trait("CP", "CP-10")]
    [Trait("CP", "CP-11")]
    [Trait("CP", "CP-18")]
    [Trait("Nivel", "Funcional")]
    public async Task FlujoCompleto_EscenarioE01_EjecutaCicloDeVidaCompletoYVerificaOraculoYBaseDeDatos()
    {
        // -------------------------------------------------------------------------------------------------
        // Escenario E-01:
        // 1. Alta de producto P-001 (Tornillo 1/4)
        // 2. Entrada 1: 10 @ $20.00 (Fecha T1)
        // 3. Entrada 2: 10 @ $30.00 (Fecha T2)
        // 4. Intentos de rechazo con verificación de no-modificación en BD:
        //    - Salida por 25 u. (CP-07: existencia insuficiente)
        //    - Entrada con fecha retroactiva < T2 (CP-20: regla temporal)
        // 5. Salida 1: 5 u. (CP-04 / CP-08 / CP-09)
        // 6. Consulta dual PEPS vs Promedio (CP-10)
        // 7. Auditoría final directa en base de datos
        // -------------------------------------------------------------------------------------------------
        await EjecutarFlujoE01Async();
    }

    [Fact]
    [Trait("CP", "CP-15")]
    [Trait("CP", "CP-16")]
    [Trait("Nivel", "Funcional")]
    public async Task FlujoCompleto_EscenarioE02_EjecutaAgotamientoMulticapaResiduoD03YVerificaBD()
    {
        // -------------------------------------------------------------------------------------------------
        // Escenario E-02:
        // 1. Alta de producto P-002 (Arandela 1/2)
        // 2. Entrada 1: 3 @ $10.00
        // 3. Entrada 2: 4 @ $11.00
        // 4. Salida 1: 5 u. (Multicapa PEPS: consume 3 de Capa 1 y 2 de Capa 2 - CP-15)
        // 5. Salida 2: 2 u. (Agotamiento exacto: stock llega a 0, valor $0.00 con ajuste D-03 - CP-16)
        // 6. Auditoría directa en BD con contexto desacoplado
        // -------------------------------------------------------------------------------------------------
        await EjecutarFlujoE02Async();
    }

    private Task EjecutarFlujoE01Async()
    {
        _ = fixture;
        throw new NotImplementedException(
            "Fase RED (SI-036): Orquestación de flujo completo de inicio a fin E-01 pendiente de implementar en la fase GREEN.");
    }

    private Task EjecutarFlujoE02Async()
    {
        _ = fixture;
        throw new NotImplementedException(
            "Fase RED (SI-036): Orquestación de flujo completo de inicio a fin E-02 pendiente de implementar en la fase GREEN.");
    }
}
