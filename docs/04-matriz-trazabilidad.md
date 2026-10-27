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
| CP-01 | Integración | Partición / Humo | `SmokeDatabaseTests`, `ProductoServiceIntegrationTests` | `evidencias/SI-020/Registro_Productos.md` |
| CP-02 | Integración | Partición | `ProductoServiceIntegrationTests`, `ValidadorProductoTests` | `evidencias/SI-020/Registro_Productos.md` |
| CP-03 | Integración + Funcional | Escenario | `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-024/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-04 | Integración + Funcional | Tabla de decisión | `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-025/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-05 | Unitaria + Integración + Funcional | Valores límite | `ValidadorCantidadTests`, `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-012/Validacion_Cantidades.md`, `evidencias/SI-024/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-06 | Unitaria + Integración + Funcional | Valores límite | `ValidadorCostoUnitarioTests`, `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-013/Validacion_Costo_Unitario.md`, `evidencias/SI-024/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-07 | Unitaria + Integración + Funcional | Valores límite + Tabla | `ValidadorExistenciaTests`, `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-014/Green_Test.md`, `evidencias/SI-025/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-08 | Unitaria + Funcional | Escenario E-01 | `CalculadoraPepsTests` | `evidencias/SI-017/Green_Test.md` |
| CP-09 | Unitaria + Funcional | Escenario E-01 | `CalculadoraPromedioPonderadoTests` | `evidencias/SI-016/Promedio_Ponderado_Salidas.md` |
| CP-10 | Funcional | Escenario E-01 | | `evidencias/CP-10/` |
| CP-11 | Funcional / Integración | Tabla de decisión | `ProductoServiceIntegrationTests`, `InventarioServiceIntegrationTests` | `evidencias/SI-021/Consulta_Listado_Productos.md`, `evidencias/SI-024/Green_Test.md` |
| CP-12 | Integración + manual | Escenario | | `evidencias/CP-12/` |
| CP-13 | Integración | Escenario | | `evidencias/CP-13/` |
| CP-14 | Regresión | — | Suite completa | `evidencias/ciclos/` |
| CP-15 | Unitaria | Escenario E-02 | `CalculadoraPepsTests` | `evidencias/SI-018/Green_Test.md` |
| CP-16 | Unitaria + Integración + Funcional | Valores límite | `ValidadorExistenciaTests`, `CalculadoraPepsTests`, `InventarioServiceIntegrationTests` | `evidencias/SI-014/Green_Test.md`, `evidencias/SI-018/Green_Test.md`, `evidencias/SI-025/Green_Test.md` |
| CP-17 | Unitaria | Escenario E-03 | `PoliticaRedondeoTests`, `CalculadoraPromedioPonderadoTests`, `CalculadoraPepsTests` | `evidencias/SI-016/Promedio_Ponderado_Salidas.md`, `evidencias/SI-018/Green_Test.md` |
| CP-18 | Integración + Funcional | Tabla de decisión | `InventarioServiceIntegrationTests`, `MovimientosPagesTests` | `evidencias/SI-024/Green_Test.md`, `evidencias/SI-030/Interfaz_Entradas_Salidas.md` |
| CP-19 | Integración / Regresión | Escenario / Auditoría | `Producto_Schema_DebeTenerPrecisionDecimalCorrecta` | `evidencias/DEF-001/` |

## 3. RNF-05 · Componentes y responsabilidades (completar al cierre)
| Componente | Responsabilidad | Probado por |
|---|---|---|
| `PoliticaRedondeo` | Redondeo bancario de costos unitarios e importes | CP-17 |
| `ValidadorCantidad`, `ValidadorCostoUnitario` | RB-01, RB-02, RB-03 | CP-05, CP-06, CP-07 |
| `ValidadorExistencia` | RB-03 (salida <= existencia disponible) | CP-07, CP-16 |
| `ValidadorProducto` | RF-01, RNF-03 (código, nombre, unidad obligatorios, longitudes y validación en modificación) | CP-02, base CP-18 |
| `ValidadorMovimiento` | RB-01, RB-02, RB-03 | CP-05, CP-06, CP-07 |
| `CalculadoraPeps` | Creación y consumo de capas | CP-08, CP-15 |
| `CalculadoraPromedioPonderado` | Promedio móvil, costo de salida y ajuste por residuo D-03 | CP-09, CP-17 |
| `ProductoService` | RF-01, RF-05, RB-10 (alta, consulta, modificación, estado) | CP-01, CP-02, CP-11, base CP-18 |
| `InventarioService` | RF-02, RF-03, RF-05 con transacción atómica | CP-03, CP-04, CP-05, CP-06, CP-07, CP-11, CP-15, CP-16, CP-18 |
| `Pages/Productos/` (`Index`, `Crear`, `Editar`) | RF-01, RF-05, RNF-03, RB-10 (interfaz de productos) | CP-01, CP-02, CP-11 |
| `Pages/Movimientos/` (`Entrada`, `Salida`) | RF-02, RF-03, RF-04, RNF-03, RB-10 (interfaz de movimientos) | CP-03, CP-04, CP-05, CP-06, CP-07, CP-18 |
