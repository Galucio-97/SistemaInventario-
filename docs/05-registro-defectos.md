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
| DEF-002 | Pérdida de portabilidad por acoplamiento de cadena de conexión a SQLEXPRESS local en appsettings.Development.json | Alta | Revisión estática / Ejecución en macOS | Corregido | Def002PortabilidadConexionTests |

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

## DEF-002 — Pérdida de portabilidad en SI.Web por acoplamiento de cadena de conexión a SQLEXPRESS local

| Campo | Valor |
|---|---|
| Fecha / Detectó | 2026-10-29 / Riguidix (Dev A / Tech Lead) · Verificado: 2026-10-29 |
| Técnica de detección | Revisión estática de commits en `dev` + Quiebre de arranque en entorno macOS / Docker |
| Requisito violado | RNF-02 (Persistencia y portabilidad), RNF-05 (Configuración externa 12-Factor) |
| Descripción | En commit `d4f68b1`, se reemplazó la cadena de conexión estándar en `SI.Web/appsettings.Development.json` por la instancia local de Windows (`Server=localhost\SQLEXPRESS;Integrated Security=True`), y en commit `07076f6` se introdujo un ternario acoplado al SO (`OperatingSystem.IsWindows()`) en `TestDbContextFixture.cs`. Además se commiteó el archivo personal `SistemaInventario.sln.DotSettings.user` |
| Pasos para reproducir | 1. Descargar commit `d4f68b1` en entorno macOS/Linux o levantar con Docker. 2. Ejecutar la aplicación web `SI.Web`. 3. Observar fallo de conexión a SQL Server por buscar `localhost\SQLEXPRESS` con autenticación Windows en lugar del contenedor Docker estándar `localhost,1433` |
| Esperado | Configuración compartida neutra y portable apuntando a `localhost,1433` con autenticación SQL para contenedores Docker/CI/macOS; configuraciones específicas de desarrollador gestionadas fuera de Git mediante `dotnet user-secrets` o variables de entorno |
| Obtenido | Error de conexión por acoplamiento exclusivo al SQL Express instalado localmente en la máquina Windows del Dev B |
| Impacto | Bloqueo de ejecución de la aplicación web en macOS, Linux y entornos Dockerizados; riesgo de fallas en CI; exposición de rutas locales personales en el repositorio |
| Severidad | Alta |
| Evidencia | `evidencias/DEF-002/Red_Test.md` (fallo RED) y `evidencias/DEF-002/Green_Test.md` (aprobación GREEN) |
| Causa | Falta de segregación entre configuración base del equipo y configuración de máquina local; violación del flujo de secrets en ASP.NET Core y push directo a `dev` sin PR |
| Corrección | Revertir commit `d4f68b1` (`fc25ed1`), desrastrear `SistemaInventario.sln.DotSettings.user` (`bd59058`), eliminar ternario acoplado al SO en `TestDbContextFixture.cs` restaurando fallback neutro, y agregar prueba automatizada de regresión `Def002PortabilidadConexionTests` |
| Regresión | `SI.Tests.RegressionTests.Def002PortabilidadConexionTests` en verde (GREEN) |

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
