# 01 · Plan de Pruebas y Cronograma

## 1. Alcance de las pruebas

Se prueban RF-01 a RF-05, RNF-01 a RNF-06 y RB-01 a RB-11 (ver `02-requisitos-y-decisiones.md`). No se prueban módulos fuera de alcance, rendimiento ni seguridad.

## 2. Niveles de prueba

| Nivel | Objeto | Ambiente | Carpeta en `SI.Tests` |
|---|---|---|---|
| Unitarias | Validadores, `PoliticaRedondeo`, calculadoras PEPS y Promedio (funciones puras) | Memoria, sin BD | `Unitarias/` |
| Integración | Servicios + EF Core + SQL Server: persistencia, transacciones, índice único | `SistemaInventarioDb_Tests` | `Integracion/` |
| Funcionales | Flujos completos vía servicios + ejecución manual en la UI con capturas | BD de pruebas / app local | `Funcionales/` |
| Regresión | Suite completa + 1 prueba por defecto corregido | Igual que arriba | `Regresion/` |

## 3. Flujo de trabajo TDD (D-07)

```
1. Elegir CP → 2. Leer oráculo (03-diseno) → 3. Escribir prueba
4. Ejecutar: debe FALLAR (RED)  → commit  "test(CP-XX): red - <descripción>"
5. Implementar lo mínimo        → ejecutar: PASA (GREEN) → commit "feat(...): CP-XX green"
6. Refactor con suite en verde  → PR a dev
```

- Los commits RED viven **solo en la rama `feat/*`**. A `dev` solo se integra con la suite en verde.
- El historial de commits RED → GREEN es **evidencia del proceso TDD** para el informe.

### ¿Cuándo un RED es un defecto?

| Situación | ¿Defecto? |
|---|---|
| Prueba en RED porque la funcionalidad aún no existe (paso 4 de TDD) | **No.** Es el ciclo normal |
| Prueba que estaba en GREEN pasa a RED tras otro cambio | **Sí** (regresión) |
| Prueba falla después de que el desarrollador declaró la funcionalidad terminada | **Sí** |
| La ejecución manual en la UI o la verificación en BD contradice el oráculo | **Sí** |
| Hallazgo en revisión de código o de esquema que viola un requisito (ej. DEF-001) | **Sí** |

## 4. Etiquetado de pruebas (trazabilidad, RNF-06)

```csharp
[Trait("CP", "CP-08")] [Trait("RF", "RF-04")] [Trait("Nivel", "Unitaria")]
```

```bash
dotnet test --filter "CP=CP-08"                 # un caso
dotnet test --filter "Nivel=Integracion"        # un nivel
dotnet test --logger "trx;LogFileName=ciclo1.trx" --results-directory evidencias/ciclos
```

## 5. Ambiente

- SQL Server en Docker (`compose.yaml`). Imagen objetivo: `mcr.microsoft.com/mssql/server:2022-latest` (ver H-05).
- BD de app: `SistemaInventarioDb` · BD de pruebas: `SistemaInventarioDb_Tests` (se recrea al inicio de la suite de integración; cada prueba limpia sus datos).
- **Prohibido** usar EF InMemory para pruebas de transacciones (no soporta transacciones → CP-13 sería un falso positivo).

## 6. Criterios de entrada y salida

| Ciclo | Entrada | Salida |
|---|---|---|
| Ciclo 1 (ejecución) | Feature freeze alcanzado · fichas con resultado esperado numérico · BD de pruebas operativa | 100% de CP ejecutados, resultado obtenido y evidencia registrados |
| Ciclo 2 (regresión) | Defectos críticos/altos corregidos con prueba de regresión | Suite completa ejecutada · defectos abiertos declarados con justificación |

## 7. Roles (2 integrantes)

| Rol | Integrante | Responsabilidades |
|---|---|---|
| **Dev A — Núcleo** (más experiencia en C#) | _Por asignar_ | Esquema/EF, motor PEPS, servicio transaccional, pruebas de integración |
| **Dev B — Producto y QA** | _Por asignar_ | Validadores, redondeo, promedio, servicio y UI de productos, UI de movimientos, fichas y evidencias |
| **Revisión cruzada** | Ambos | Todo PR lo revisa el otro integrante; cada uno **ejecuta manualmente los CP del código del otro** (independencia del tester) |

## 8. Cronograma (sáb 17 → vie 23 de octubre de 2026)

| Día | Dev A — Núcleo | Dev B — Producto y QA | Hito |
|---|---|---|---|
| **D1 · Sáb 17** | Corregir DEF-001 (H-02), H-01, H-05; referencia `SI.Tests → SI.Web` (H-03); entidades `Movimiento`, `CapaPeps`; migración (TICKET-01/02) | Revisar oráculos E-01/02/03 a mano; TDD de `PoliticaRedondeo` y `ValidadorMovimiento` (TICKET-05/10 · CP-05, CP-06) | **G0** cerrado |
| **D2 · Dom 18** | TDD `CalculadoraPeps` (TICKET-07 · CP-08, CP-15) | TDD `CalculadoraPromedioPonderado` (TICKET-06 · CP-09, CP-17) | Motor de costeo en verde |
| **D3 · Lun 19** | BD de pruebas + TDD `InventarioService` entradas/salidas con transacción (TICKET-08 · CP-03, CP-04, CP-07, CP-13) | TDD `ProductoService` (TICKET-03 · CP-01, CP-02, CP-11, CP-18) | Servicios en verde |
| **D4 · Mar 20** | Consulta de inventario por método (CP-10) · persistencia (CP-12) · invariante (CP-19) · límite (CP-16) | UI de productos (TICKET-04) · UI de entradas y salidas (TICKET-09) | — |
| **D5 · Mié 21** | Pruebas funcionales vía servicios (TICKET-13) · apoyo a UI de consulta | UI de consulta/Kardex · **inicio Ciclo 1**: ejecución manual con capturas | 🔒 **Feature freeze 18:00** |
| **D6 · Jue 22** | Corrección de defectos + prueba de regresión por defecto | Cierre Ciclo 1 · registro de defectos · **Ciclo 2 (regresión, CP-14)** | **G2** |
| **D7 · Vie 23** | Alinear README con estructura real (H-04) · instrucciones de ejecución · ZIP · prueba en clon limpio | Informe PDF (resultados reales, evidencias, trazabilidad) | 📦 **Entrega** |

### Plan de recorte (si el D4 cierra con retraso)

En este orden, sin tocar los CP mínimos:
1. Botón "Cargar ejercicio de referencia" en la UI.
2. Vista comparativa dual (se deja el selector PEPS/Promedio).
3. CP-16 y CP-18 pasan de automatizados a solo ejecución manual documentada.

**Nunca se recorta:** CP-01 a CP-14, la verificación en BD, el registro de defectos ni la regresión.

## 9. Riesgos principales

| Riesgo | Mitigación |
|---|---|
| Oráculos calculados con el propio código (prueba circular) | Oráculos en `03-diseno` calculados a mano antes del RED |
| Redondeo bancario mal verificado en Excel (`ROUND` es half-up) | Casos con 5 exacto se calculan a mano (tabla 5.1 de `02`) |
| Cronograma de 7 días sin holgura | Feature freeze fijo el D5 + plan de recorte |
| Imagen `azure-sql-edge` retirada | Migrar a `mssql/server:2022` el D1 |
