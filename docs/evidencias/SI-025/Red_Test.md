# Evidencia Fase RED — SI-025: Registrar Salida con Transacción (CP-04, CP-05, CP-07, CP-11, CP-15, CP-16, CP-18)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~RegistrarSalidaAsync"
```

## Salida obtenida (RED)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.73]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_CuandoExistenciaEsInsuficiente_RechazaYNoModificaBaseDeDatos(cantidadSalida: 20, existenciaDisponible: 15) [FAIL]
[xUnit.net 00:00:00.75]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_CuandoExistenciaEsInsuficiente_RechazaYNoModificaBaseDeDatos(cantidadSalida: 15.0001, existenciaDisponible: 15) [FAIL]
[xUnit.net 00:00:00.82]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_SalidaIgualATodaLaExistencia_AceptaDejaExistenciaCeroYValoresEnCero [FAIL]
[xUnit.net 00:00:00.84]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_SalidaValidaEscenarioE01_S1_ConsumeCapasCalculaCostosAmbosMetodosYActualizaProductoEnTransaccion [FAIL]
[xUnit.net 00:00:00.86]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_EscenarioE02_SalidaMulticapa_ConsumeCapasEnOrdenYCalculaCostos [FAIL]

  Con error SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_CuandoExistenciaEsInsuficiente_RechazaYNoModificaBaseDeDatos(cantidadSalida: 20, existenciaDisponible: 15) [596 ms]
  Mensaje de error:
   Assert.Contains() Failure: Sub-string not found
String:    "No implementado"
Not found: "Existencia insuficiente"

  Con error SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_SalidaIgualATodaLaExistencia_AceptaDejaExistenciaCeroYValoresEnCero [14 ms]
  Mensaje de error:
   Assert.True() Failure
Expected: True
Actual:   False

  Con error SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_SalidaValidaEscenarioE01_S1_ConsumeCapasCalculaCostosAmbosMetodosYActualizaProductoEnTransaccion [14 ms]
  Mensaje de error:
   Assert.True() Failure
Expected: True
Actual:   False

  Con error SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_EscenarioE02_SalidaMulticapa_ConsumeCapasEnOrdenYCalculaCostos [14 ms]
  Mensaje de error:
   Assert.True() Failure
Expected: True
Actual:   False

Con error! - Con error:     5, Superado:     5, Omitido:     0, Total:    10, Duración: 123 ms - SI.Tests.dll (net8.0)
```

## Resumen del fallo
- Las pruebas fallan sobre la base de datos real `SistemaInventarioDb_Tests` porque `InventarioService.RegistrarSalidaAsync` aún no implementa la validación de suficiencia de existencia (`ValidadorExistencia`), el consumo FIFO de capas (`CalculadoraPeps.CalcularSalida`), la aplicación del costo promedio (`CalculadoraPromedioPonderado.CalcularSalida`), ni la persistencia atómica en transacción del movimiento y producto.
- Casos diseñados y fallando como RED:
  - **CP-04 (`RF-03`, `RB-07`):** Salida S1 de E-01 (5 unidades) que consume de la capa más antigua, calcula costos en ambos métodos (PEPS 100.00 / Promedio 125.00) y actualiza saldos (PEPS 400.00 / Promedio 375.00).
  - **CP-16 (`RB-03`):** Salida que agota toda la existencia (15 unidades) dejando saldo y existencia en 0.00.
  - **CP-07 (`RB-03`):** Rechazo cuando la salida supera la existencia física disponible (`ValidadorExistencia`).
  - **CP-15 (`RF-03`, `RB-07`):** Salida multicapa en Escenario E-02 consumiendo capas en estricto orden cronológico.
