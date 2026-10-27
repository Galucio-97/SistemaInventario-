# Evidencia Fase GREEN — SI-026: Atomicidad ante fallos (RNF-04, RB-06, CP-13)

## Comando ejecutado (Pruebas de Rollback)
```bash
dotnet test --filter "FullyQualifiedName~TransaccionRollbackTests"
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

Correctas! - Con error:     0, Superado:     2, Omitido:     0, Total:     2, Duración: 54 ms - SI.Tests.dll (net8.0)
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

Correctas! - Con error:     0, Superado:   119, Omitido:     0, Total:   119, Duración: 1 s - SI.Tests.dll (net8.0)
```

## Verificación de Atomicidad y Estado en Base de Datos (CP-13)

### 1. Salida con fallo simulado en commit
- **Precondición:** Se registraron `E1` (10 unidades @ $20.00) y `E2` (10 unidades @ $30.00). El producto alcanzó:
  - `ExistenciaActual`: 20.0000
  - `CostoPromedioActual`: 25.0000
  - `ValorInventarioPromedio`: 500.00
  - `ValorInventarioPeps`: 500.00
  - 2 capas PEPS (10 y 10 disponibles)
  - 2 movimientos tipo Entrada
- **Operación ejecutada con interceptor:** `RegistrarSalidaAsync(productoId, fecha, 5.0000m)`.
  - El interceptor `SimuladorFalloTransaccionInterceptor` lanzó `InvalidOperationException` al interceptar `TransactionCommittingAsync`.
  - La transacción de base de datos fue descartada (`await using var transaction` ejecutó `DisposeAsync` en estado no confirmado, provocando `ROLLBACK TRANSACTION` en SQL Server).
- **Inspección en `DbContext` nuevo e independiente:**
  - `SELECT ExistenciaActual, CostoPromedioActual, ValorInventarioPromedio, ValorInventarioPeps FROM Productos WHERE Id = @id;`
    - Resultado: `20.0000 | 25.0000 | 500.00 | 500.00`
  - `SELECT CantidadDisponible, CantidadInicial, CostoUnitario FROM CapasPeps WHERE ProductoId = @id ORDER BY FechaEntrada;`
    - Resultado: 2 registros intactos `(10.0000, 10.0000, 20.0000)` y `(10.0000, 10.0000, 30.0000)`. Ninguna capa fue consumida.
  - `SELECT COUNT(*) FROM Movimientos WHERE ProductoId = @id;`
    - Resultado: 2 movimientos (ambos `Entrada`). Ningún movimiento de `Salida` fue persistido.

### 2. Entrada con fallo simulado en commit
- **Operación ejecutada con interceptor:** `RegistrarEntradaAsync(productoId, fecha, 10.0000m, 20.0000m)`.
  - El primer `SaveChangesAsync` generó el registro preliminar de movimiento con su Id en el buffer de la transacción SQL Server.
  - Al ejecutar `TransactionCommittingAsync`, se lanzó la excepción simulada.
- **Inspección en `DbContext` nuevo e independiente:**
  - `Productos`: Existencia 0.0000, saldos en 0.00.
  - `CapasPeps`: 0 capas registradas.
  - `Movimientos`: 0 movimientos registrados.

## Conclusión
Se demuestra el cumplimiento riguroso de:
- **RNF-04 (Consistencia y atomicidad transaccional)**: ningún fallo a mitad de la operación deja datos parciales ni estados corruptos.
- **RB-06 (Rechazo sin efectos colaterales)**: las operaciones fallidas revierten completamente cualquier cambio en BD.
- **CP-13 (Atomicidad ante fallos)**: validado al 100% en integración real con SQL Server 2022.
