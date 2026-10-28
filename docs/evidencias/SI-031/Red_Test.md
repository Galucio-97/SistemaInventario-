# Evidencia de Fase RED — SI-031: Consulta de existencias por método de costeo

- **Ticket:** SI-031
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-031-Consulta-Existencias-Metodo`
- **Requisitos cubiertos:** RF-04, RF-05, CP-10, CP-11, D-09, RB-04, RNF-01

## 1. Pruebas Diseñadas

En [`ConsultaExistenciasIntegrationTests.cs`](file:///C:/Users/gaalz/OneDrive/Documents/GitHub/SistemaInventario/SI.Tests/IntegrationTests/ConsultaExistenciasIntegrationTests.cs):
1. `ConsultarExistenciasAsync_CuandoProductoNoExiste_RetornaNoEncontrado` (CP-11, RF-05)
2. `ConsultarExistenciasAsync_EscenarioE01_MetodoPromedio_CalculaExistenciaYValorCorrectos` (CP-10, RF-04, RF-05)
3. `ConsultarExistenciasAsync_EscenarioE01_MetodoPeps_CalculaExistenciaValorYCapasCorrectas` (CP-10, RF-04, RF-05, D-09)
4. `ConsultarExistenciasAsync_MetodoPeps_CuandoExistenciaEsCero_RetornaCostoCero` (D-09)
5. `ConsultarExistenciasAsync_NoModificaProductosMovimientosNiCapas` (RF-05, RNF-01, RB-04)

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
[xUnit.net] SI.Tests.IntegrationTests.ConsultaExistenciasIntegrationTests.ConsultarExistenciasAsync_CuandoProductoNoExiste_RetornaNoEncontrado [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.ConsultaExistenciasIntegrationTests.ConsultarExistenciasAsync_EscenarioE01_MetodoPromedio_CalculaExistenciaYValorCorrectos [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.ConsultaExistenciasIntegrationTests.ConsultarExistenciasAsync_NoModificaProductosMovimientosNiCapas [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.ConsultaExistenciasIntegrationTests.ConsultarExistenciasAsync_EscenarioE01_MetodoPeps_CalculaExistenciaValorYCapasCorrectas [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.ConsultaExistenciasIntegrationTests.ConsultarExistenciasAsync_MetodoPeps_CuandoExistenciaEsCero_RetornaCostoCero [FAIL]

Con error! - Con error: 5, Superado: 0, Omitido: 0, Total: 5
System.NotImplementedException: Método no implementado aún (Fase RED).
```

Estado: **ROJO CONFIRMADO**
