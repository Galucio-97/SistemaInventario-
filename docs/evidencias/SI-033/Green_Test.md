# Evidencia de Pruebas — SI-033: Interfaz de consulta de inventario e historial

- **Ticket:** SI-033
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-033-Interfaz-Consulta-Inventario-Historial`
- **Requisitos cubiertos:** RF-04, RF-05, CP-08, CP-09, CP-10, CP-11 (manual y automatizado)

## 1. Pruebas Funcionales Automatizadas (`ConsultaInventarioHistorialPageTests.cs` y `ConsultaExistenciasPageTests.cs`)

1. `ConsultaModel_OnGetAsync_ConProductoSeleccionado_CargaHistorialMovimientosOrdenado` -> **PASÓ**
   - Carga el historial ordenado cronológicamente con sus 3 movimientos del Escenario E-01.
2. `ConsultaModel_OnGetAsync_MetodoPeps_CargaHistorialConCostosPepsYCapasActivas` -> **PASÓ**
   - Verifica salida con costo unitario $20.00, costo total $100.00, saldo valor $400.00 y 2 capas PEPS activas.
3. `ConsultaModel_OnGetAsync_MetodoPromedio_CargaHistorialConCostosPromedio` -> **PASÓ**
   - Verifica salida con costo unitario $25.00, costo total $125.00, saldo valor $375.00.
4. `ConsultaModel_OnGetAsync_SinProductoSeleccionado_CargaListaYResultadoNulo` -> **PASÓ**
5. `ConsultaModel_OnGetAsync_ProductoInexistente_EstableceMensajeError` -> **PASÓ**

## 2. Resultado de la Ejecución de Pruebas

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
Correctas! - Con error: 0, Superado: 148, Omitido: 0, Total: 148, Duración: 4 s
Estado: 100% de éxito en toda la suite de pruebas del sistema.
```
