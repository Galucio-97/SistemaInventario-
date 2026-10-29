# Evidencia de Fase GREEN — SI-032: Historial de movimientos por método

- **Ticket:** SI-032
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-032-Historial-Movimientos-Metodo`
- **Requisitos cubiertos:** RF-04, RF-05, CP-10, CP-11, RB-04, RNF-01

## 1. Pruebas Ejecutadas (`HistorialMovimientosIntegrationTests.cs`)

1. `ObtenerHistorialMovimientosAsync_CuandoProductoNoExiste_RetornaNoEncontrado` (CP-11, RF-05) -> **PASÓ**
2. `ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPromedio_CoincideFilaPorFilaConOraculo` (CP-10, RF-04, RF-05) -> **PASÓ**
   - Fila 1 (Entrada 10 @ $20.00): CostoTotal $200.00, Existencia 10.0000, Saldo $200.00
   - Fila 2 (Entrada 10 @ $30.00): CostoTotal $300.00, Existencia 20.0000, Saldo $500.00
   - Fila 3 (Salida 5 u): CostoUnitario $25.0000, CostoTotal $125.0000, Existencia 15.0000, Saldo $375.0000
3. `ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPeps_CoincideFilaPorFilaConOraculo` (CP-10, RF-04, RF-05) -> **PASÓ**
   - Fila 1 (Entrada 10 @ $20.00): CostoTotal $200.00, Existencia 10.0000, Saldo $200.00
   - Fila 2 (Entrada 10 @ $30.00): CostoTotal $300.00, Existencia 20.0000, Saldo $500.00
   - Fila 3 (Salida 5 u de Capa 1): CostoUnitario $20.0000, CostoTotal $100.0000, Existencia 15.0000, Saldo $400.0000
4. `ObtenerHistorialMovimientosAsync_CambioMetodo_NoAlteraFilasFechasNiCantidades` (Criterio 3 de SI-032) -> **PASÓ**
   - El número de movimientos, fechas, tipos, cantidades y existencias físicas es idéntico entre ambos métodos.
5. `ObtenerHistorialMovimientosAsync_OperacionLectura_NoGeneraEfectosSecundarios` (RNF-01, RB-04, RF-05) -> **PASÓ**
   - Operación pura de lectura con `.AsNoTracking()`, sin mutaciones en base de datos.

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
Correctas! - Con error: 0, Superado: 5, Omitido: 0, Total: 5, Duración: 138 ms
Suite completa del sistema: 145 / 145 pruebas superadas (100% éxito)
```

Estado: **VERDE CONFIRMADO**
