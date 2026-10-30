# Evidencia de Prueba RED — DEF-002

## 1. Justificación Técnica y de Negocio

El sistema está diseñado para ser desplegado y desarrollado de forma multiplataforma y portable (**RNF-02**, **RNF-05**), utilizando como estándar común el contenedor Docker oficial de SQL Server 2022 en `localhost,1433`.

En el commit `d4f68b1`, el desarrollador Dev B (Gustavo) sobrescribió directamente en la rama compartida `dev` las cadenas de conexión en `SI.Web/appsettings.Development.json` por la instancia local de su equipo Windows:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SistemaInventarioDb;Integrated Security=True;..."
```
Adicionalmente, en el commit `07076f6` dentro del PR #94, se introdujo una bifurcación en `TestDbContextFixture.cs` acoplada al sistema operativo (`OperatingSystem.IsWindows()`), asumiendo erróneamente que cualquier entorno Windows debía forzar `SQLEXPRESS`.

### Impacto de la Regresión
1. **Quiebre en macOS, Linux y Contenedores:** Al iniciar la aplicación web en entornos fuera de Windows o basados en Docker, el proveedor SQL Server falla con excepciones de red (`SocketException` / `SqlException`), imposibilitando la conexión.
2. **Violación de 12-Factor App:** La configuración compartida en Git debe ser neutra y portable; las particularidades de la máquina de cada desarrollador deben inyectarse mediante User Secrets o variables de entorno.
3. **Exposición de archivos locales:** Se versionó `SistemaInventario.sln.DotSettings.user` con rutas absolutas locales del usuario.

---

## 2. Ficha Técnica del Defecto DEF-002

| Campo | Valor |
|---|---|
| **ID** | `DEF-002` |
| **Título** | Pérdida de portabilidad en `SI.Web` por acoplamiento de cadena de conexión a SQLEXPRESS local en `appsettings.Development.json` |
| **Fecha / Detectó** | 2026-10-29 / Riguidix (Dev A / Tech Lead) |
| **Técnica de detección** | Revisión estática de commits en `dev` + Quiebre de arranque en entorno macOS / Docker |
| **Requisito violado** | **RNF-02** (Persistencia y portabilidad), **RNF-05** (Arquitectura limpia y configuración 12-Factor) |
| **Severidad** | **Alta** (impide la ejecución del backend web en entornos fuera de la máquina del autor) |
| **Estado** | Confirmado (RED verificado) |
| **Commit causante** | `d4f68b1` (push directo a `dev`) y `07076f6` |

---

## 3. Salida de Ejecución RED (Fallo de Regresión)

Al evaluar la prueba de regresión `Def002PortabilidadConexionTests` sobre el estado afectado del commit `d4f68b1`:

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.45]     SI.Tests.RegressionTests.Def002PortabilidadConexionTests.AppSettingsDevelopment_DebeMantenerConfiguracionPortableDocker [FAIL]
  Mensaje de error:
   Assert.DoesNotContain() Failure
   Found:    SQLEXPRESS
   In:       Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true
  Seguimiento de la pila:
     at SI.Tests.RegressionTests.Def002PortabilidadConexionTests.AppSettingsDevelopment_DebeMantenerConfiguracionPortableDocker() in SI.Tests/RegressionTests/Def002PortabilidadConexionTests.cs:line 27

[xUnit.net 00:00:00.47]     SI.Tests.RegressionTests.Def002PortabilidadConexionTests.TestDbContextFixture_DebeTenerFallbackNeutroSinAcoplamientoASqlExpress [FAIL]
  Mensaje de error:
   Assert.DoesNotContain() Failure
   Found:    SQLEXPRESS
   In:       Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true
  Seguimiento de la pila:
     at SI.Tests.RegressionTests.Def002PortabilidadConexionTests.TestDbContextFixture_DebeTenerFallbackNeutroSinAcoplamientoASqlExpress() in SI.Tests/RegressionTests/Def002PortabilidadConexionTests.cs:line 45

Con error! - Con error: 2, Superado: 0, Omitido: 0, Total: 2, Duración: 112 ms - SI.Tests.dll (net8.0)
```
