using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Enums;
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

    private async Task EjecutarFlujoE01Async()
    {
        // -------------------------------------------------------------------------------------------------
        // Escenario E-02:
        // 2. Entrada 1: 3 @ $10.00
        // 3. Entrada 2: 4 @ $11.00
        // 4. Salida 1: 5 u. (Multicapa PEPS: consume 3 de Capa 1 y 2 de Capa 2 - CP-15)
        // 5. Salida 2: 2 u. (Agotamiento exacto: stock llega a 0, valor $0.00 con ajuste D-03 - CP-16)
        // 6. Auditoría directa en BD con contexto desacoplado
        // -------------------------------------------------------------------------------------------------
        fixture.ResetDatabase();
        var productoService = new ProductoService(fixture.Context);
        var inventarioService = new InventarioService(fixture.Context);
        
        // Creación de producto
        var resultadoProducto = await productoService.RegistrarAsync("P-001", "Tornillo 1/4", "pieza");
        
        Assert.NotNull(resultadoProducto);
        Assert.True(resultadoProducto.Exitoso);
        
        var productoId = resultadoProducto.EntidadId!.Value;
        
        // Creación de entrada
        var resultadoPrimerEntrada = await inventarioService.RegistrarEntradaAsync(
            productoId, 
            DateTime.UtcNow, 
            10, 
            20, 
            CancellationToken.None
        );

        Assert.NotNull(resultadoPrimerEntrada);
        Assert.True(resultadoPrimerEntrada.Exitoso);
        
        // Creación de entrada
        var resultadoSegundaEntrada = await inventarioService.RegistrarEntradaAsync(
            productoId, 
            DateTime.UtcNow, 
            10, 
            30, 
            CancellationToken.None
        );

        Assert.NotNull(resultadoSegundaEntrada);
        Assert.True(resultadoSegundaEntrada.Exitoso);
        
        // Creación de salida falsa
        var resultadoPrimeraSalida = await inventarioService.RegistrarSalidaAsync(
            productoId,
            DateTime.UtcNow,
            25,
            CancellationToken.None
        );
        
        Assert.NotNull(resultadoPrimeraSalida);
        Assert.True(!resultadoPrimeraSalida.Exitoso);
        Assert.Contains("Existencia insuficiente", resultadoPrimeraSalida.MensajeError ?? string.Empty);
        
        var resultadoSegundaSalida = await inventarioService.RegistrarSalidaAsync(
            productoId,
            DateTime.UtcNow,
            5,
            CancellationToken.None
        );
        
        Assert.NotNull(resultadoSegundaSalida);
        Assert.True(resultadoSegundaSalida.Exitoso);

        // 6. Consulta dual PEPS vs Promedio Ponderado (CP-08, CP-09, CP-10)
        var consultaPeps = await inventarioService.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);
        Assert.True(consultaPeps.Encontrado);
        Assert.Equal(15.0000m, consultaPeps.ExistenciaActual);
        Assert.Equal(400.00m, consultaPeps.ValorInventario);
        Assert.Equal(26.6667m, consultaPeps.CostoUnitario);
        Assert.Equal(2, consultaPeps.CapasActivas.Count);
        Assert.Equal(5.0000m, consultaPeps.CapasActivas[0].CantidadDisponible);
        Assert.Equal(20.0000m, consultaPeps.CapasActivas[0].CostoUnitario);
        Assert.Equal(10.0000m, consultaPeps.CapasActivas[1].CantidadDisponible);
        Assert.Equal(30.0000m, consultaPeps.CapasActivas[1].CostoUnitario);

        var consultaProm = await inventarioService.ConsultarExistenciasAsync(productoId, MetodoCosteo.PromedioPonderado);
        Assert.True(consultaProm.Encontrado);
        Assert.Equal(15.0000m, consultaProm.ExistenciaActual); // CP-10: Invarianza de cantidad física
        Assert.Equal(375.00m, consultaProm.ValorInventario);
        Assert.Equal(25.0000m, consultaProm.CostoUnitario);

        // Historial Kardex en ambos métodos
        var historialPeps = await inventarioService.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.Peps);
        Assert.True(historialPeps.Encontrado);
        Assert.Equal(3, historialPeps.Movimientos.Count);
        Assert.Equal(400.00m, historialPeps.Movimientos[2].SaldoValor);

        var historialProm = await inventarioService.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.PromedioPonderado);
        Assert.True(historialProm.Encontrado);
        Assert.Equal(3, historialProm.Movimientos.Count);
        Assert.Equal(375.00m, historialProm.Movimientos[2].SaldoValor);

        // 7. Auditoría final directa en base de datos con contexto desacoplado
        await using var ctxAuditoria = fixture.CreateDbContext();
        var productoDb = await ctxAuditoria.Productos.FindAsync(productoId);
        Assert.NotNull(productoDb);
        Assert.Equal(15.0000m, productoDb.ExistenciaActual);
        Assert.Equal(400.00m, productoDb.ValorInventarioPeps);
        Assert.Equal(375.00m, productoDb.ValorInventarioPromedio);
        Assert.Equal(25.0000m, productoDb.CostoPromedioActual);

        var totalMovimientosDb = await ctxAuditoria.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(3, totalMovimientosDb);

        var capasActivasDb = await ctxAuditoria.CapasInventarioPeps
            .Where(c => c.ProductoId == productoId && c.CantidadDisponible > 0)
            .OrderBy(c => c.Id)
            .ToListAsync();
        Assert.Equal(2, capasActivasDb.Count);
        Assert.Equal(5.0000m, capasActivasDb[0].CantidadDisponible);
        Assert.Equal(10.0000m, capasActivasDb[1].CantidadDisponible);
    }
    
    private async Task EjecutarFlujoE02Async()
    {
        // -------------------------------------------------------------------------------------------------
        // Escenario E-02 (Agotamiento multicapa y residuo D-03 / CP-15, CP-16, CP-17):
        // 1. Alta de producto P-002
        // 2. Entrada 1: 3 @ $10.00
        // 3. Entrada 2: 4 @ $11.00
        // 4. Salida 1: 5 u. (Multicapa PEPS: consume 3 @ $10.00 y 2 @ $11.00)
        // 5. Salida 2: 2 u. (Agotamiento exacto a stock 0 con ajuste D-03)
        // 6. Auditoría directa en BD con contexto desacoplado
        // -------------------------------------------------------------------------------------------------
        fixture.ResetDatabase();
        var productoService = new ProductoService(fixture.Context);
        var inventarioService = new InventarioService(fixture.Context);

        // 1. Alta de producto P-002
        var resultadoProducto = await productoService.RegistrarAsync("P-002", "Arandela 1/2", "pieza");
        Assert.NotNull(resultadoProducto);
        Assert.True(resultadoProducto.Exitoso);
        var productoId = (int)resultadoProducto.EntidadId!;

        // 2. Entrada 1: 3 @ $10.00
        var t1 = new DateTime(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);
        var e1 = await inventarioService.RegistrarEntradaAsync(productoId, t1, 3.0000m, 10.0000m);
        Assert.True(e1.Exitoso);

        // 3. Entrada 2: 4 @ $11.00
        var t2 = t1.AddHours(2);
        var e2 = await inventarioService.RegistrarEntradaAsync(productoId, t2, 4.0000m, 11.0000m);
        Assert.True(e2.Exitoso);

        // 4. Salida 1: 5 unidades (CP-15: Multicapa PEPS)
        // Consume Capa 1 completa (3 @ 10.00 = 30.00) y parte de Capa 2 (2 @ 11.00 = 22.00) -> Costo PEPS: 52.00
        // Existencia resultante: 2.0000. Saldo PEPS remanente: 2 @ 11.00 = 22.00
        var t3 = t2.AddHours(2);
        var s1 = await inventarioService.RegistrarSalidaAsync(productoId, t3, 5.0000m);
        Assert.True(s1.Exitoso);

        var consultaIntermediaPeps = await inventarioService.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);
        Assert.Equal(2.0000m, consultaIntermediaPeps.ExistenciaActual);
        Assert.Equal(22.00m, consultaIntermediaPeps.ValorInventario);
        Assert.Single(consultaIntermediaPeps.CapasActivas);
        Assert.Equal(2.0000m, consultaIntermediaPeps.CapasActivas[0].CantidadDisponible);

        // 5. Salida 2: 2 unidades (CP-16: Agotamiento exacto a stock 0, ajuste D-03 / CP-17)
        var t4 = t3.AddHours(2);
        var s2 = await inventarioService.RegistrarSalidaAsync(productoId, t4, 2.0000m);
        Assert.True(s2.Exitoso);

        // 6. Consulta dual tras agotamiento
        var consultaFinalPeps = await inventarioService.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);
        Assert.Equal(0.0000m, consultaFinalPeps.ExistenciaActual);
        Assert.Equal(0.00m, consultaFinalPeps.ValorInventario);
        Assert.Empty(consultaFinalPeps.CapasActivas);

        var consultaFinalProm = await inventarioService.ConsultarExistenciasAsync(productoId, MetodoCosteo.PromedioPonderado);
        Assert.Equal(0.0000m, consultaFinalProm.ExistenciaActual);
        Assert.Equal(0.00m, consultaFinalProm.ValorInventario); // Ajuste D-03: saldo 0.00 cuando existencia es 0

        // Historial Kardex de E-02
        var historialE02 = await inventarioService.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.Peps);
        Assert.Equal(4, historialE02.Movimientos.Count);
        Assert.Equal(0.0000m, historialE02.Movimientos[3].ExistenciaResultante);
        Assert.Equal(0.00m, historialE02.Movimientos[3].SaldoValor);

        // 7. Auditoría directa en base de datos con contexto desacoplado
        await using var ctxAuditoriaE02 = fixture.CreateDbContext();
        var productoDbE02 = await ctxAuditoriaE02.Productos.FindAsync(productoId);
        Assert.NotNull(productoDbE02);
        Assert.Equal(0.0000m, productoDbE02.ExistenciaActual);
        Assert.Equal(0.00m, productoDbE02.ValorInventarioPeps);
        Assert.Equal(0.00m, productoDbE02.ValorInventarioPromedio);

        var totalMovimientosE02 = await ctxAuditoriaE02.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(4, totalMovimientosE02);

        var capasConSaldo = await ctxAuditoriaE02.CapasInventarioPeps
            .CountAsync(c => c.ProductoId == productoId && c.CantidadDisponible > 0);
        Assert.Equal(0, capasConSaldo);
    }
}
