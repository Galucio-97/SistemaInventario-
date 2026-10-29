# Evidencia de Fase RED — SI-032: Historial de movimientos por método

- **Ticket:** SI-032
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-032-Historial-Movimientos-Metodo`
- **Requisitos cubiertos:** RF-04, RF-05, CP-10, CP-11, RB-04, RNF-01

## 1. Pruebas Diseñadas

En [`HistorialMovimientosIntegrationTests.cs`](file:///C:/Users/gaalz/OneDrive/Documents/GitHub/SistemaInventario/SI.Tests/IntegrationTests/HistorialMovimientosIntegrationTests.cs):
1. `ObtenerHistorialMovimientosAsync_CuandoProductoNoExiste_RetornaNoEncontrado` (CP-11, RF-05)
2. `ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPromedio_CoincideFilaPorFilaConOraculo` (CP-10, RF-04, RF-05)
3. `ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPeps_CoincideFilaPorFilaConOraculo` (CP-10, RF-04, RF-05)
4. `ObtenerHistorialMovimientosAsync_CambioMetodo_NoAlteraFilasFechasNiCantidades` (Criterio 3 de SI-032)
5. `ObtenerHistorialMovimientosAsync_OperacionLectura_NoGeneraEfectosSecundarios` (RNF-01, RB-04, RF-05)

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
[xUnit.net] SI.Tests.IntegrationTests.HistorialMovimientosIntegrationTests.ObtenerHistorialMovimientosAsync_OperacionLectura_NoGeneraEfectosSecundarios [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.HistorialMovimientosIntegrationTests.ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPromedio_CoincideFilaPorFilaConOraculo [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.HistorialMovimientosIntegrationTests.ObtenerHistorialMovimientosAsync_CambioMetodo_NoAlteraFilasFechasNiCantidades [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.HistorialMovimientosIntegrationTests.ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPeps_CoincideFilaPorFilaConOraculo [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.HistorialMovimientosIntegrationTests.ObtenerHistorialMovimientosAsync_CuandoProductoNoExiste_RetornaNoEncontrado [FAIL]

Con error! - Con error: 5, Superado: 0, Omitido: 0, Total: 5, Duración: 146 ms
System.NotImplementedException: Método no implementado aún (Fase RED - SI-032).
```

Estado: **ROJO CONFIRMADO**
