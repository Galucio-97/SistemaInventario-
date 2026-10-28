# Evidencia Fase RED — SI-028: Invariante de valores almacenados (RNF-01, RF-05, RB-08, CP-19)

## Objetivo de la fase RED
Diseñar y formular la suite de pruebas de integración `InvarianteValoresTests` para dar cumplimiento exhaustivo a `CP-19` (`RNF-01`, `RF-05`, `RB-08`), garantizando matemáticamente que tras **cada** movimiento de inventario los valores guardados en el producto jamás diverjan de las capas PEPS ni del historial de movimientos (`H-06`).

## Casos de prueba diseñados
1. **`Invariante_SeCumpleTrasCadaMovimiento_EscenarioE01` (CP-19):**
   - Ejecuta secuencialmente los movimientos de `E-01` (`E1`: 10 @ $20.00, `E2`: 10 @ $30.00, `S1`: 5 u.).
   - Tras cada movimiento individual, verifica en BD que:
     - `Producto.ValorInventarioPeps == Σ (CantidadDisponible × CostoUnitario)` de las capas PEPS vigentes.
     - `Producto.ValorInventarioPeps == Movimiento.SaldoValorPeps` del último movimiento.
     - `Producto.ValorInventarioPromedio == Movimiento.SaldoValorPromedio` del último movimiento.
     - `Producto.ExistenciaActual == Σ entradas − Σ salidas` del historial completo.
     - `Producto.ExistenciaActual == Movimiento.ExistenciaResultante` del último movimiento.
2. **`Invariante_SeCumpleTrasCadaMovimiento_EscenarioE02` (CP-19):**
   - Ejecuta secuencialmente los 4 movimientos de `E-02` (`E1`: 3 @ $10.00, `E2`: 4 @ $11.00, `S1`: 5 multicapa, `S2`: 2 agotamiento total a 0 con ajuste `D-03`).
   - Verifica el invariante tras cada operación, garantizando coherencia al agotar capas y existencias.
3. **`Invariante_FallaSiSeAlteraCualquieraDeLasTresIgualdades`:**
   - Comprueba que si se introduce una discrepancia artificial en el valor PEPS, en el valor Promedio o en la Existencia del producto respecto a sus capas o historial, la aserción del invariante detecta la corrupción y lanza `XunitException`.
