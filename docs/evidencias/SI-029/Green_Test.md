# Evidencia Fase GREEN — SI-029: Rechazo de fechas retroactivas (RB-11, D-04, CP-20)

## Comando ejecutado (Pruebas del Caso CP-20)
```bash
dotnet test --filter "FullyQualifiedName~FechaMovimiento|FullyQualifiedName~CuandoFechaEs"
```

## Salida de pruebas (GREEN)
```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     8, Omitido:     0, Total:     8, Duración: 495 ms - SI.Tests.dll (net8.0)
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

Correctas! - Con error:     0, Superado:   140, Omitido:     0, Total:   140, Duración: 1 s - SI.Tests.dll (net8.0)
```

## Verificación de Resultados (`CP-20`, `RB-11`, `D-04`)

### 1. Validación en Dominio (`ValidadorFechaMovimiento`)
- **Fecha posterior:** Aceptada sin error.
- **Misma fecha:** Aceptada sin error.
- **Fecha anterior:** Rechazada con mensaje explícito: *"La fecha del movimiento no puede ser anterior al último movimiento registrado."*.
- **Sin movimientos previos:** Aceptada para el primer movimiento del producto.

### 2. Validación de Integración y Transaccionalidad (`InventarioService`)
- **Entrada retroactiva:** Rechazada antes de persistir; la base de datos permanece idéntica (conteo de movimientos, existencia y valores inalterados).
- **Entrada con misma fecha:** Aceptada y registrada correctamente actualizando capas y existencias.
- **Salida retroactiva:** Rechazada antes de descontar inventario o consumir capas PEPS; la base de datos permanece intacta.
- **Salida con misma fecha:** Aceptada y registrada correctamente descontando capas y existencia.
