# Evidencia Fase GREEN — SI-027: Persistencia tras reinicio (RNF-02, RB-05, CP-12)

## Comando ejecutado (Prueba de Persistencia)
```bash
dotnet test --filter "FullyQualifiedName~PersistenciaTests"
```

## Salida de pruebas (GREEN)
```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     1, Omitido:     0, Total:     1, Duración: < 1 ms - SI.Tests.dll (net8.0)
```

## Comando ejecutado (Suite Completa)
```bash
dotnet test
```

## Salida Suite Completa
```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   129, Omitido:     0, Total:   129, Duración: 2 s - SI.Tests.dll (net8.0)
```

## Verificación de Resultados (`CP-12` / Escenario `E-01`)

### 1. Verificación Automática (Desacoplamiento total de contextos)
Tras desechar los contextos de base de datos de cada operación individual (`Creación P-001`, `Entrada E1`, `Entrada E2`, `Salida S1`), se instanció un nuevo `DbContext` limpio y se comprobó que los datos leídos directamente de SQL Server coinciden al 100% con el oráculo de `E-01`:

- **Producto (`P-001`):**
  - `ExistenciaActual`: 15.0000
  - `CostoPromedioActual`: 25.0000
  - `ValorInventarioPromedio`: 375.00
  - `ValorInventarioPeps`: 400.00
- **Capas PEPS:**
  - Capa 1: Fecha `2026-10-01 09:00`, Inicial `10.0000`, Disponible `5.0000`, Costo `20.0000`.
  - Capa 2: Fecha `2026-10-02 09:00`, Inicial `10.0000`, Disponible `10.0000`, Costo `30.0000`.
- **Movimientos:**
  - `E1`: Fecha `2026-10-01 09:00`, Cantidad `10.0000`, Costo Entrada `20.0000`, Saldo PEPS `200.00`, Saldo Promedio `200.00`, Existencia `10.0000`.
  - `E2`: Fecha `2026-10-02 09:00`, Cantidad `10.0000`, Costo Entrada `30.0000`, Saldo PEPS `500.00`, Saldo Promedio `500.00`, Existencia `20.0000`.
  - `S1`: Fecha `2026-10-03 10:00`, Cantidad `5.0000`, Costo Salida PEPS `100.00` (Saldo `400.00`), Costo Salida Promedio `125.00` (Saldo `375.00`), Existencia `15.0000`.

### 2. Procedimiento de Verificación Manual (Ficha CP-12)
1. Ejecutar la aplicación web: `dotnet run --project SI.Web`.
2. Registrar o verificar el producto `P-001` y sus movimientos del escenario `E-01` en la interfaz.
3. Detener la aplicación web presionando `Ctrl+C` en la terminal.
4. Volver a arrancar la aplicación: `dotnet run --project SI.Web`.
5. Navegar a la vista de productos (`/Productos`) y verificar que `P-001` se muestra con los mismos valores de existencia y saldos.
