# Documentación de Funcionalidad — SI-031: Consulta de existencias por método de costeo

## Resumen del Ticket
- **Identificador:** SI-031
- **Versión:** v0.5.0
- **Requisitos:** RF-04, RF-05, CP-10, CP-11, D-09, RB-04, RNF-01
- **Componentes:**
  - `SI.Web.Enums.MetodoCosteo`: Enum para `Peps` (1) y `PromedioPonderado` (2).
  - `SI.Web.Common.ResultadoConsultaExistencias` y `CapaPepsDto`: DTO inmutable con factory methods.
  - `SI.Web.Interfaces.IInventarioService`: Declaración de `ConsultarExistenciasAsync`.
  - `SI.Web.Services.InventarioService`: Implementación de solo lectura con `.AsNoTracking()`.
  - `SI.Web.Pages.Inventario.Consulta`: Razor Page interactiva con filtros, tarjetas KPI, comparativa dual PEPS vs Promedio y tabla de capas FIFO activas.

## Escenario E-01 (CP-10) Validado
- **Entrada 1:** 10 piezas @ $20.00 -> $200.00
- **Entrada 2:** 10 piezas @ $30.00 -> $300.00
- **Salida:** 5 piezas
- **Resultado PEPS:**
  - Existencia: 15.0000 piezas
  - Capa 1 restante: 5 piezas @ $20.00 = $100.00
  - Capa 2 restante: 10 piezas @ $30.00 = $300.00
  - Valuación PEPS: $400.0000
  - Costo Unitario Efectivo (D-09): $26.6667 (400.0000 / 15.0000)
- **Resultado Promedio Ponderado:**
  - Existencia: 15.0000 piezas
  - Costo Unitario Promedio: $25.0000
  - Valuación Promedio: $375.0000
- **Diferencia de Valuación:** +$25.00 en PEPS respecto a Promedio Ponderado.

## Idempotencia y Seguridad (RF-05, RNF-01, RB-04)
- La consulta no genera transacciones ni escrituras.
- Todas las consultas a EF Core utilizan `.AsNoTracking()`.
- Validaciones defensivas contra productos inexistentes o nulos (CP-11).
