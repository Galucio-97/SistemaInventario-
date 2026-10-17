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
| DEF-001 | Costo promedio persistido con 2 decimales | Alta | Revisión estática | Nuevo | CP-17, CP-19 |

---

## DEF-001 — Costo promedio persistido con 2 decimales en lugar de 4

| Campo | Valor |
|---|---|
| Fecha / Detectó | 2026-10-16 / PM (revisión de esquema) |
| Técnica de detección | Revisión estática de la migración `20261016215721_InitialMigration` |
| Requisito violado | RB-08, RB-09 (política §5.2: costo unitario a 4 decimales) |
| Descripción | `Productos.CostoPromedioActual` se generó como `decimal(18,2)` porque `ProductoConfiguration` no define `HasPrecision`. Además `ExistenciaActual` es `int`, incompatible con D-01 |
| Pasos para reproducir | 1. Abrir `SI.Web/Data/Migrations/20261016215721_InitialMigration.cs`, línea 25. 2. Observar `type: "decimal(18,2)"` para `CostoPromedioActual` |
| Esperado | `CostoPromedioActual decimal(18,4)`; `ExistenciaActual decimal(18,4)`; importes `decimal(18,2)` declarados explícitamente |
| Obtenido | `CostoPromedioActual decimal(18,2)`; `ExistenciaActual int` |
| Impacto | En E-02 el promedio 10.5714 se guardaría como 10.57 → la salida de 5 unidades costaría 52.85 en lugar de **52.86** |
| Evidencia | `evidencias/DEF-001/migracion-antes.png` (captura de la línea 25) |
| Causa | Falta de `HasPrecision` en la configuración de EF; EF aplica `decimal(18,2)` por defecto |
| Corrección | _pendiente_ — `HasPrecision(18,4)` / `HasPrecision(18,2)` en `ProductoConfiguration` + nueva migración. Commit: _hash_ |
| Regresión | _pendiente_ — CP-17 y CP-19 en verde; prueba de esquema que verifique la precisión de columnas |

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
