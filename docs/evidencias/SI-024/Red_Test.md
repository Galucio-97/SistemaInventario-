# Evidencia Fase RED — SI-024: Registrar Entrada con Transacción (CP-03, CP-05, CP-06, CP-11, CP-18)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~InventarioServiceIntegrationTests"
```

## Salida obtenida (RED)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.47]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoProductoNoExiste_RechazaConMensajeYNoModificaBaseDeDatos [FAIL]
[xUnit.net 00:00:00.58]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos(cantidadInvalida: 10.12345, mensajeEsperado: "La cantidad no puede tener más de 4 decimales.") [FAIL]
[xUnit.net 00:00:00.59]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos(cantidadInvalida: -1, mensajeEsperado: "La cantidad debe ser mayor que cero.") [FAIL]
[xUnit.net 00:00:00.60]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos(cantidadInvalida: 0, mensajeEsperado: "La cantidad debe ser mayor que cero.") [FAIL]
[xUnit.net 00:00:00.60]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCostoUnitarioEsInvalido_RechazaYNoModificaBaseDeDatos(costoInvalido: 20.12345, mensajeEsperado: "El costo unitario no puede tener más de 4 decimales.") [FAIL]
[xUnit.net 00:00:00.61]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCostoUnitarioEsInvalido_RechazaYNoModificaBaseDeDatos(costoInvalido: 0, mensajeEsperado: "El costo unitario debe ser mayor que cero.") [FAIL]
[xUnit.net 00:00:00.62]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoCostoUnitarioEsInvalido_RechazaYNoModificaBaseDeDatos(costoInvalido: -5, mensajeEsperado: "El costo unitario debe ser mayor que cero.") [FAIL]
[xUnit.net 00:00:00.63]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_EntradaValida_CreaMovimientoCapaYActualizaProductoEnTransaccion [FAIL]
[xUnit.net 00:00:00.63]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoProductoEstaInactivo_RechazaConMensajeYNoModificaBaseDeDatos [FAIL]
[xUnit.net 00:00:00.64]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_MultiplesEntradasConsecutivas_AcumulaExistenciasCapasYActualizaPromedioPonderado [FAIL]

Con error! - Con error:    10, Superado:     0, Omitido:     0, Total:    10, Duración: 171 ms - SI.Tests.dll (net8.0)
```

## Resumen del fallo
- Las 10 pruebas fallan de manera natural sobre la base de datos real `SistemaInventarioDb_Tests` porque `InventarioService.RegistrarEntradaAsync` está actualmente en forma de stub (`new ResultadoOperacion(false, "No implementado")`).
- Se verifican las precondiciones y aserciones para:
  - **CP-03 (`RF-02`, `RB-04`, `RB-05`):** Creación atómica de `Movimiento`, `CapasPeps` y actualización de `Producto` con valores del escenario E-01 E1 (10 @ 20.00 $\rightarrow$ 200.00 PEPS / 200.00 Promedio).
  - **CP-05 (`RB-01`):** Rechazo de cantidades negativas, cero o $> 4$ decimales.
  - **CP-06 (`RB-02`):** Rechazo de costos unitarios negativos, cero o $> 4$ decimales.
  - **CP-11 (`RB-04`):** Rechazo ante producto inexistente (`"El producto no existe"`).
  - **CP-18 (`RB-10`):** Rechazo ante producto inactivo (`"El producto está inactivo"`).
  - Verificación de entradas consecutivas acumulativas (E1 + E2) con promedio ponderado continuo.
