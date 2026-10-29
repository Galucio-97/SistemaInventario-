# Evidencia de Fase GREEN — SI-031: Consulta de existencias por método de costeo

- **Ticket:** SI-031
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-031-Consulta-Existencias-Metodo`
- **Requisitos cubiertos:** RF-04, RF-05, CP-10, CP-11, D-09, RB-04, RNF-01

## 1. Pruebas Ejecutadas

### Pruebas de Integración (`ConsultaExistenciasIntegrationTests.cs`)
1. `ConsultarExistenciasAsync_CuandoProductoNoExiste_RetornaNoEncontrado` (CP-11, RF-05) -> **PASÓ**
2. `ConsultarExistenciasAsync_EscenarioE01_MetodoPromedio_CalculaExistenciaYValorCorrectos` (CP-10, RF-04, RF-05) -> **PASÓ**
   - Existencia: 15.0000
   - Costo Promedio: $25.0000
   - Valuación Promedio: $375.0000
3. `ConsultarExistenciasAsync_EscenarioE01_MetodoPeps_CalculaExistenciaValorYCapasCorrectas` (CP-10, RF-04, RF-05, D-09) -> **PASÓ**
   - Existencia: 15.0000
   - Costo Unitario PEPS (D-09): $26.6667 (400 / 15 redondeado a 4 decimales)
   - Valuación PEPS: $400.0000
   - Capas Activas: 2 capas ([5.0000 @ $20.0000, 10.0000 @ $30.0000])
4. `ConsultarExistenciasAsync_MetodoPeps_CuandoExistenciaEsCero_RetornaCostoCero` (D-09) -> **PASÓ**
5. `ConsultarExistenciasAsync_NoModificaProductosMovimientosNiCapas` (RF-05, RNF-01, RB-04) -> **PASÓ**

### Pruebas Funcionales Web (`ConsultaExistenciasPageTests.cs`)
6. `ConsultaModel_OnGetAsync_SinProductoSeleccionado_CargaListaYResultadoNulo` -> **PASÓ**
7. `ConsultaModel_OnGetAsync_ProductoInexistente_EstableceMensajeError` (CP-11) -> **PASÓ**
8. `ConsultaModel_OnGetAsync_ConProductoYMovimientos_CargaResultadoYComparativo` (CP-10, D-09) -> **PASÓ**

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
Correctas! - Con error: 0, Superado: 8, Omitido: 0, Total: 8, Duración: 1 s
Suite completa del sistema: 140 / 140 pruebas superadas (100% éxito)
```

Estado: **VERDE CONFIRMADO**
