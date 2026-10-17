# 04 · Matriz de Trazabilidad (RNF-06)

> Se actualiza al cerrar cada ciclo. La columna *Prueba automatizada* contiene el nombre del método xUnit; el filtro `dotnet test --filter "CP=CP-XX"` la ejecuta.

## 1. Requisito → Casos

| Requisito | CP | C1 | C2 |
|---|---|---|---|
| RF-01 Productos | CP-01, CP-02, CP-11, CP-18 | | |
| RF-02 Entradas | CP-03, CP-05, CP-06 | | |
| RF-03 Salidas | CP-04, CP-05, CP-07, CP-16 | | |
| RF-04 Métodos de costeo | CP-08, CP-09, CP-10, CP-15, CP-17 | | |
| RF-05 Existencias e historial | CP-10, CP-11, CP-19 | | |
| RNF-01 Integridad | CP-17, CP-19 | | |
| RNF-02 Persistencia | CP-01, CP-12 | | |
| RNF-03 Validación | CP-02, CP-05, CP-06, CP-07 | | |
| RNF-04 Consistencia | CP-07, CP-13 | | |
| RNF-05 Mantenibilidad | Revisión de estructura (sección 3) | | |
| RNF-06 Trazabilidad | Este documento + etiquetas `[Trait]` | | |
| RB-01 Cantidad > 0 | CP-05 | | |
| RB-02 Costo > 0 | CP-06 | | |
| RB-03 Salida ≤ existencia | CP-07, CP-16 | | |
| RB-04 Movimiento ↔ producto existente | CP-03, CP-11 | | |
| RB-05 Persistencia de confirmados | CP-03, CP-12 | | |
| RB-06 Rechazo sin efectos | CP-05, CP-06, CP-07, CP-13 | | |
| RB-07 Costo según método | CP-04, CP-08, CP-09 | | |
| RB-08 Precisión decimal | CP-17, CP-19 | | |
| RB-09 Redondeo documentado | CP-17 | | |
| RB-10 Producto inactivo | CP-18 | | |
| RB-11 Fecha retroactiva | _por diseñar si se confirma D-04_ | | |

## 2. Caso → Nivel → Prueba automatizada → Evidencia

| CP | Nivel | Técnica | Prueba automatizada | Evidencia |
|---|---|---|---|---|
| CP-01 | Integración | Partición | | `evidencias/CP-01/` |
| CP-02 | Integración | Partición | | `evidencias/CP-02/` |
| CP-03 | Funcional | Escenario | | `evidencias/CP-03/` |
| CP-04 | Funcional | Tabla de decisión | | `evidencias/CP-04/` |
| CP-05 | Unitaria + Integración | Valores límite | | `evidencias/CP-05/` |
| CP-06 | Unitaria | Valores límite | | `evidencias/CP-06/` |
| CP-07 | Funcional | Valores límite + Tabla | | `evidencias/CP-07/` |
| CP-08 | Unitaria + Funcional | Escenario E-01 | | `evidencias/CP-08/` |
| CP-09 | Unitaria + Funcional | Escenario E-01 | | `evidencias/CP-09/` |
| CP-10 | Funcional | Escenario E-01 | | `evidencias/CP-10/` |
| CP-11 | Funcional | Tabla de decisión | | `evidencias/CP-11/` |
| CP-12 | Integración + manual | Escenario | | `evidencias/CP-12/` |
| CP-13 | Integración | Escenario | | `evidencias/CP-13/` |
| CP-14 | Regresión | — | Suite completa | `evidencias/ciclos/` |
| CP-15 | Unitaria | Escenario E-02 | | `evidencias/CP-15/` |
| CP-16 | Funcional | Valores límite | | `evidencias/CP-16/` |
| CP-17 | Unitaria | Escenario E-03 | | `evidencias/CP-17/` |
| CP-18 | Funcional | Tabla de decisión | | `evidencias/CP-18/` |
| CP-19 | Integración / Regresión | Escenario / Auditoría | `Producto_Schema_DebeTenerPrecisionDecimalCorrecta` | `evidencias/DEF-001/` |

## 3. RNF-05 · Componentes y responsabilidades (completar al cierre)

| Componente | Responsabilidad | Probado por |
|---|---|---|
| `PoliticaRedondeo` | Redondeo bancario de costos unitarios e importes | CP-17 |
| `ValidadorMovimiento` | RB-01, RB-02, RB-03 | CP-05, CP-06, CP-07 |
| `CalculadoraPeps` | Creación y consumo de capas | CP-08, CP-15 |
| `CalculadoraPromedioPonderado` | Promedio móvil y costo de salida | CP-09, CP-17 |
| `ProductoService` | RF-01 | CP-01, CP-02, CP-11 |
| `InventarioService` | RF-02, RF-03, RF-05 con transacción | CP-03, CP-04, CP-13 |
