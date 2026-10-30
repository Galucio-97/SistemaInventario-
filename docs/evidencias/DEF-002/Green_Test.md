# Evidencia de Prueba GREEN — DEF-002

## 1. Resumen de la Corrección (TDD: RED → GREEN)

- **Defecto Asociado:** `DEF-002` — *Pérdida de portabilidad en `SI.Web` por acoplamiento de cadena de conexión a SQLEXPRESS local en `appsettings.Development.json`*.
- **Requisitos Satisfechos:** **RNF-02** (Persistencia y portabilidad), **RNF-05** (Configuración externa 12-Factor).
- **Resultado:** Se revirtió el commit `d4f68b1`, se restauró la cadena de conexión portable estándar en `SI.Web/appsettings.Development.json` (`localhost,1433`), se eliminó el acoplamiento a sistema operativo en `TestDbContextFixture.cs`, se desrastreó `SistemaInventario.sln.DotSettings.user`, y la suite de regresión pasó exitosamente (**GREEN**).

---

## 2. Cambios Implementados

### 2.1 Reversión de `d4f68b1` y Restauración de `appsettings.Development.json`
Se restauraron las cadenas de conexión estándar del equipo apuntando al contenedor Docker multiplataforma:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SistemaInventarioDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "TestConnection": "Server=localhost,1433;Database=SistemaInventarioDb_Tests;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### 2.2 Desacoplamiento de `TestDbContextFixture.cs`
Se eliminó la bifurcación condicional por sistema operativo (`OperatingSystem.IsWindows()`), manteniendo la lectura de variable de entorno con fallback neutro y universal:

```csharp
// Cadena de conexión configurable mediante variable de entorno (utilizada en CI / local)
public static string ConnectionString =>
    Environment.GetEnvironmentVariable("ConnectionStrings__TestConnection")
    ?? "Server=localhost,1433;Database=SistemaInventarioDb_Tests;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true";
```

### 2.3 Desrastreo de Archivos de Usuario
Se desrastreó `SistemaInventario.sln.DotSettings.user` del índice de Git para garantizar el cumplimiento de la regla `*.DotSettings.user` en `.gitignore`.

### 2.4 Instrucción para Desarrolladores con SQLEXPRESS Local
Para entornos locales específicos (ej. Windows con SQLEXPRESS nativo), se documenta el uso de User Secrets fuera del versionado de Git:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

---

## 3. Salida de Ejecución GREEN (Regresión Aprobada)

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.RegressionTests.Def002PortabilidadConexionTests.AppSettingsDevelopment_DebeMantenerConfiguracionPortableDocker [< 1 ms]
  Correctas SI.Tests.RegressionTests.Def002PortabilidadConexionTests.TestDbContextFixture_DebeTenerFallbackNeutroSinAcoplamientoASqlExpress [< 1 ms]
  Correctas SI.Tests.RegressionTests.Def001PrecisionProductoTests.Producto_Schema_DebeTenerPrecisionDecimalCorrecta [25 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 3
     Correcto: 3
     Fallidas: 0
 Tiempo total: 0.3200 Segundos
```
