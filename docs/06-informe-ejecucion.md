# 06 · Informe de Ejecución de Pruebas

> Este documento registra los **resultados reales de ejecución** obtenidos en el repositorio y la suite de pruebas. Constituye el insumo directo para el **Entregable 3 (Informe de Pruebas en PDF)**.

---

## 1. Resumen Ejecutivo de Calidad

* **Estado General:** Fase 0 (Foundation / `v0.0.1`) completada y promovida a `main`. Fase 1 (Datos y Esquema / `v0.1.0`) en ejecución activa.
* **Tag de Línea Base (`main`):** `v0.0.1` (Commit `1ae5a48706c177c2d0169d7ed3da1c030f436a48`).
* **Rama de Trabajo Actual:** `feat/SI-007-Modelo_de_Datos_Movimientos` (tras completar y verificar `SI-006`).
* **Defectos Críticos/Altos Detectados y Gestionados:** 1 defecto real (`DEF-001`, severidad Alta, corregido y verificado con prueba de regresión).
* **Infraestructura de Pruebas:** Automatización xUnit integrada en pipeline CI mediante GitHub Actions (`.github/workflows/ci.yml`).

---

## 2. Ambiente de Ejecución Real

| Parámetro | Valor Verificado en Entorno Local | Valor en Entorno de Integración Continua (CI) |
|---|---|---|
| **Sistema Operativo** | macOS 26.5 (Darwin 24.3.0 ARM64 / Apple Silicon) | Ubuntu Latest (Linux x86_64) |
| **.NET SDK** | `8.0.426` (C# 12 / .NET 8 LTS) | `8.0.x` (instalado vía `actions/setup-dotnet@v4`) |
| **Motor de Base de Datos** | SQL Server 2022 en Docker (`mcr.microsoft.com/mssql/server:2022-latest`) | Contenedor SQL Server / Service Container en GitHub Runner |
| **Herramienta de Pruebas** | xUnit 2.5.3 + VSTest 17.11.1 | `dotnet test --no-build --verbosity normal` |
| **Medición de Cobertura** | Coverlet Collector 6.0.0 | Habilitado para reporte de métricas |
| **Commit Evaluado** | `1ae5a48` (Tag `v0.0.1`) / `27e1a05` (`SI-006` en `feat/SI-007`) | Pipeline CI en pull requests hacia `dev`, `beta` y `main` |

---

## 3. Métricas de Progreso por Ciclo

| Métrica | Fase 0 (`v0.0.1`) | Ciclo 1 (Funcional / Integración) | Ciclo 2 (Regresión Final) |
|---|:---:|:---:|:---:|
| **Fecha de Corte** | 17-oct-2026 | *En progreso (Día 1-5)* | *Programado (Día 6)* |
| **Casos Diseñados Totales** | 19 casos | 19 casos | 19 casos |
| **Casos Ejecutados** | 2 casos preliminares (`CP-19` y base) | — | — |
| **Casos Aprobados (Pass)** | 2 | — | — |
| **Casos Fallidos (Fail)** | 0 (post-fix DEF-001) | — | — |
| **Casos Bloqueados / Pendientes** | 17 pendientes de implementación | — | — |
| **Defectos Detectados Totales** | 1 (`DEF-001`) | — | — |
| **Defectos Corregidos y Verificados** | 1 (`DEF-001`) | — | — |
| **Pruebas Automatizadas en Verde** | 100% de las suites activas | — | — |

---

## 4. Estado Detallado por Caso de Prueba (Matriz de Ejecución)

| ID | Requisito | Nivel | Técnica de Diseño | Estado Actual | Prueba Automatizada Asociada | Evidencia Documentada |
|:---|:---|:---:|:---:|:---:|:---|:---|
| **CP-01** | RF-01, RNF-02 | Integración | Partición de Equivalencia | *Pendiente (v0.3.0)* | `ProductoServiceTests.Registrar_Valido_Persiste` | `evidencias/CP-01/` |
| **CP-02** | RF-01, RNF-03 | Integración | Partición (Inválida) | *Pendiente (v0.3.0)* | `ProductoServiceTests.Registrar_ClaveDuplicada_Rechaza` | `evidencias/CP-02/` |
| **CP-03** | RF-02, RB-04 | Funcional | Basada en Escenarios | *Pendiente (v0.4.0)* | `InventarioServiceTests.Entrada_Valida_ActualizaStockYValor` | `evidencias/CP-03/` |
| **CP-04** | RF-03, RB-07 | Funcional | Tabla de Decisión (R5) | *Pendiente (v0.4.0)* | `InventarioServiceTests.Salida_Valida_CalculaCostoYDescuenta` | `evidencias/CP-04/` |
| **CP-05** | RB-01, RNF-03 | Unitaria + Int. | Valores Límite | *Pendiente (v0.2.0)* | `ValidadorMovimientoTests.Cantidad_MenorOIgualACero_Rechaza` | `evidencias/CP-05/` |
| **CP-06** | RB-02, RNF-03 | Unitaria | Valores Límite | *Pendiente (v0.2.0)* | `ValidadorMovimientoTests.CostoEntrada_MenorOIgualACero_Rechaza` | `evidencias/CP-06/` |
| **CP-07** | RB-03, RNF-04 | Funcional | Límite / Tabla (R4) | *Pendiente (v0.4.0)* | `InventarioServiceTests.Salida_MayorAExistencia_Rechaza` | `evidencias/CP-07/` |
| **CP-08** | RF-04, RB-07 | Unitaria + Func. | Escenario Obligatorio 6.3 | *Pendiente (v0.2.0)* | `CalculadoraPepsTests.Salida_EjercicioReferencia_Costo100` | `evidencias/CP-08/` |
| **CP-09** | RF-04, RB-07 | Unitaria + Func. | Escenario Obligatorio 6.3 | *Pendiente (v0.2.0)* | `CalculadoraPromedioTests.Salida_EjercicioReferencia_Costo125` | `evidencias/CP-09/` |
| **CP-10** | RF-04, RF-05 | Funcional | Escenario Obligatorio 6.3 | *Pendiente (v0.5.0)* | `ConsultaInventarioTests.CambioMetodo_RecalculaSinAlterarHistorial` | `evidencias/CP-10/` |
| **CP-11** | RF-01, RF-05 | Funcional | Tabla de Decisión (R1) | *Pendiente (v0.3.0)* | `ProductoServiceTests.Consultar_Inexistente_InformaNoExiste` | `evidencias/CP-11/` |
| **CP-12** | RNF-02, RB-05 | Integración | Basada en Escenarios | *Pendiente (v0.4.0)* | `PersistenciaTests.ReinicioAplicacion_ConservaDatos` | `evidencias/CP-12/` |
| **CP-13** | RNF-04, RB-06 | Integración | Inyección de Fallos | *Pendiente (v0.4.0)* | `TransaccionRollbackTests.FalloOperacion_HaceRollbackCompleto` | `evidencias/CP-13/` |
| **CP-14** | Todos | Regresión | Suite Completa | *Pendiente (v0.9.x)* | Re-ejecución total automatizada tras fixes | `evidencias/ciclos/` |
| **CP-15** | RF-04 | Unitaria | Escenario E-02 | *Pendiente (v0.2.0)* | `CalculadoraPepsTests.Salida_Multicapa_ConsumeEnOrdenFIFO` | `evidencias/CP-15/` |
| **CP-16** | RB-03 | Funcional | Valores Límite | *Pendiente (v0.4.0)* | `InventarioServiceTests.Salida_ExactaExistencia_DejaSaldoCero` | `evidencias/CP-16/` |
| **CP-17** | RB-08, RB-09 | Unitaria | Escenario E-03 | *Pendiente (v0.2.0)* | `PoliticaRedondeoTests.RedondeoBancario_ResiduoAjustado` | `evidencias/CP-17/` |
| **CP-18** | RF-01, RB-10 | Funcional | Tabla de Decisión (R2) | *Pendiente (v0.3.0)* | `InventarioServiceTests.Movimiento_ProductoInactivo_Rechaza` | `evidencias/CP-18/` |
| **CP-19** | RNF-01, RB-08 | Regresión / Int. | Auditoría de Esquema | **✅ APROBADO (C1)** | `Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta` | `evidencias/DEF-001/` |

---

## 5. Registro y Seguimiento de Defectos Reales

| ID | Título | Severidad | Detectado en | Estado | Causa Raíz | Commit de Fix | Prueba de Regresión |
|:---:|:---|:---:|:---:|:---:|:---|:---:|:---|
| **DEF-001** | Costo promedio persistido con 2 decimales y existencia en `int` | **Alta** | Revisión de arquitectura / esquema inicial | **✅ Corregido y Verificado** | Ausencia de `HasPrecision` en `ProductoConfiguration` y tipo CLR `int` en entidad `Producto`. Truncaba decimales en promedios móviles como 10.5714 $\rightarrow$ 10.57. | `27e1a05` (migración `20261017224554_AgregarProductos`) | `Def001PrecisionProductoTests` (GREEN) |

---

## 6. Defectos No Resueltos y Riesgos Residuales

* **Defectos Abiertos:** Ninguno al corte de la Release `v0.0.1`.
* **Riesgo Técnico Controlado:** Dependencia de la imagen Docker de SQL Server para pruebas de integración; mitigado con scripts reproducibles y runner de CI con Docker habilitado.
* **Control de Alcance:** Se mantiene la política estricta de no agregar módulos administrativos ni comerciales ajenos a la unidad de inventario y costeo.

---

## 7. Conclusiones Parciales (Fase 0 & Avance Fase 1)

1. La base de la solución (`v0.0.1`) quedó consolidada con un flujo de integración continua robusto, garantizando que ningún cambio entre a ramas protegidas sin que los tests compilen y pasen.
2. El hallazgo temprano de **DEF-001** valida la efectividad del proceso de revisión estática de la arquitectura antes de comenzar la implementación masiva de movimientos de inventario, evitando retrabajos y errores de arrastre en los cálculos de PEPS y Promedio Ponderado.
