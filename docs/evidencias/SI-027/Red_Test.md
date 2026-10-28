# Evidencia Fase RED — SI-027: Persistencia tras reinicio (RNF-02, RB-05, CP-12)

## Objetivo de la fase RED
Diseñar y formular la prueba de integración automatizada `PersistenciaTests` correspondiente a `CP-12` para validar el requisito no funcional `RNF-02` (Persistencia y durabilidad de datos) y la regla de negocio `RB-05` (Persistencia de movimientos confirmados).

## Caso de prueba formulado (`CP-12`)
- **Nombre de prueba:** `PersistenciaTests.ReinicioAplicacion_ConservaDatos_EscenarioE01Completo`
- **Técnica:** Escenario / Integración con ciclo de vida desacoplado.
- **Flujo:**
  1. **Sesión 1:** Registrar producto `P-001` y desechar el `DbContext`.
  2. **Sesión 2:** Registrar entrada `E1` (10 unidades @ $20.00) mediante `IInventarioService` en una nueva instancia de `DbContext` y desecharla.
  3. **Sesión 3:** Registrar entrada `E2` (10 unidades @ $30.00) en otra instancia independiente de `DbContext` y desecharla.
  4. **Sesión 4:** Registrar salida `S1` (5 unidades) en otra instancia independiente de `DbContext` y desecharla (simulando detención completa del proceso / servidor).
  5. **Sesión 5 (Reinicio):** Instanciar un `DbContext` nuevo e independiente desde cero (sin caché de entidades en memoria) y verificar que toda la información en SQL Server 2022 coincide con el oráculo oficial de `E-01`:
     - Producto: Existencia 15, Costo Promedio 25.0000, Saldo Promedio 375.00, Saldo PEPS 400.00.
     - Capas PEPS: Capa 1 (5 disponibles de 10 iniciales @ 20.00) y Capa 2 (10 disponibles de 10 iniciales @ 30.00).
     - Movimientos: 3 registros históricos ordenados con cantidades, tipos, costos y saldos patrimoniales exactos.
