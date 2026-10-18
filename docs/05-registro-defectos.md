# 05 · Registro de Defectos

## Flujo de estados

`Nuevo → Confirmado → En corrección → Corregido → Verificado (regresión) → Cerrado`
Alternativos: `Reabierto` (falla la regresión) · `Diferido-declarado` (no se corrige; se justifica en el informe).

## Severidad

| Severidad | Criterio |
|---|---|
| Crítica | Resultado de costeo incorrecto, pérdida o corrupción de datos, actualización parcial |
| Alta | Una regla de negocio no se cumple |
| Media | Funciona, pero con mensaje incomprensible o inconsistencia menor |
| Baja | Cosmético |

> No se inventan defectos. Un RED dentro del ciclo TDD **no** es defecto (ver `01-plan-de-pruebas.md §3`).

## Resumen

| ID | Título | Severidad | Detectado en | Estado | CP de regresión |
|---|---|---|---|---|---|
| DEF-001 | Costo promedio persistido con 2 decimales | Alta | Revisión estática | Corregido | CP-19 |

---

## DEF-001 — Costo promedio persistido con 2 decimales en lugar de 4

| Campo | Valor |
|---|---|
| Fecha / Detectó | 2026-10-16 / PM (revisión de esquema) · Verificado: 2026-10-17 |
| Técnica de detección | Revisión estática de la migración inicial + Prueba automatizada de regresión |
| Requisito violado | RB-08, RB-09 (política §5.2: costo unitario a 4 decimales), D-01 |
| Descripción | `Productos.CostoPromedioActual` se generó como `decimal(18,2)` porque `ProductoConfiguration` no definía `HasPrecision`. Además `ExistenciaActual` era `int`, incompatible con D-01 |
| Pasos para reproducir | 1. Ejecutar prueba `Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta` sobre el modelo sin configurar. 2. Observar fallo de tipo (`int` vs `decimal`) y escala |
| Esperado | `CostoPromedioActual decimal(18,4)`; `ExistenciaActual decimal(18,4)`; importes `decimal(18,2)` declarados explícitamente |
| Obtenido | Inicialmente `CostoPromedioActual decimal(18,2)`; `ExistenciaActual int` |
| Impacto | En E-02 el promedio 10.5714 se guardaría como 10.57 → la salida de 5 unidades costaría 52.85 en lugar de **52.86** |
| Evidencia | `evidencias/DEF-001/Red_Test.md` (fallo RED) y `evidencias/DEF-001/Green_Test.md` (aprobación GREEN) |
| Causa | Falta de `HasPrecision` en la configuración de EF; EF aplica `decimal(18,2)` por defecto y tipo `int` en entidad |
| Corrección | `HasPrecision(18,4)` en `ExistenciaActual` y `CostoPromedioActual`; `HasPrecision(18,2)` en importes (`ProductoConfiguration.cs`) + migración `20261017224554_AgregarProductos` |
| Regresión | `SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta` en verde (GREEN) |

---

## Plantilla

```markdown
## DEF-XXX — <Título>
| Campo | Valor |
|---|---|
| Fecha / Detectó | |
| Técnica de detección | Prueba automatizada / Ejecución manual / Revisión |
| CP / Requisito | |
| Descripción | |
| Pasos para reproducir | 1. … |
| Esperado | |
| Obtenido | |
| Severidad | |
| Evidencia | evidencias/DEF-XXX/… |
| Causa | |
| Corrección | Descripción + commit/PR |
| Regresión | Prueba agregada en Regresion/ + resultado de la suite completa |
```
