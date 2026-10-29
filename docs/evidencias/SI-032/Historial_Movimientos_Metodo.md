# Documentación de Funcionalidad — SI-032: Historial de movimientos por método

## Resumen del Ticket
- **Identificador:** SI-032
- **Versión:** v0.5.0
- **Requisitos:** RF-04, RF-05, CP-10, CP-11, RB-04, RNF-01
- **Componentes:**
  - `SI.Web.Common.ResultadoHistorialMovimientos` y `MovimientoHistorialDto`: DTO inmutable para transferir el kardex completo.
  - `SI.Web.Interfaces.IInventarioService.ObtenerHistorialMovimientosAsync`: Firma del método asíncrono.
  - `SI.Web.Services.InventarioService.ObtenerHistorialMovimientosAsync`: Implementación optimizada de solo lectura (`.AsNoTracking()`).

## Mapeo de Campos por Método de Costeo

| Campo DTO | Entradas (Ambos Métodos) | Salidas (Promedio Ponderado) | Salidas (PEPS) |
|---|---|---|---|
| `CostoUnitario` | `CostoUnitarioEntrada` | `CostoUnitarioPromedio` | `CostoUnitarioPeps` |
| `CostoTotal` | `Cantidad * CostoUnitarioEntrada` | `CostoTotalPromedio` | `CostoTotalPeps` |
| `ExistenciaResultante` | `ExistenciaResultante` | `ExistenciaResultante` | `ExistenciaResultante` |
| `SaldoValor` | `SaldoValorPromedio` | `SaldoValorPromedio` | `SaldoValorPeps` |

## Escenario E-01 (Oráculo Documento 03)
- **Fila 1 (Entrada 1):** 10 u @ $20.00 -> Costo Total $200.00, Existencia 10.0000, Saldo $200.00
- **Fila 2 (Entrada 2):** 10 u @ $30.00 -> Costo Total $300.00, Existencia 20.0000, Saldo $500.00
- **Fila 3 (Salida 1 de 5 u):**
  - **Promedio Ponderado:** Costo Unitario $25.0000, Costo Total $125.0000, Existencia 15.0000, Saldo $375.0000
  - **PEPS:** Costo Unitario $20.0000 (de Capa 1), Costo Total $100.0000, Existencia 15.0000, Saldo $400.0000
