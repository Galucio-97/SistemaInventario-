# Evidencia Fase RED — SI-018: PEPS: Salida Multicapa y Agotamiento (CP-15, CP-16, CP-17)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~CalculadoraPepsTests" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.44]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE02_Multicapa_DebeConsumirEnOrdenYCalcularCostosYSaldos [FAIL]
[xUnit.net 00:00:00.44]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE03_ConDecimalesYAgotamiento_DebeConsumirCapasYDejarSaldoCero [FAIL]
[xUnit.net 00:00:00.45]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_CuandoCapaEstaAgotada_DebeOmitirlaYConsumirDeLaSiguienteDisponible [FAIL]
[xUnit.net 00:00:00.45]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_ConCapasDeMismaFecha_DebeRespetarOrdenPorId [FAIL]
[xUnit.net 00:00:00.45]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_SalidaIgualATodaLaExistencia_DebeAgotarTodasLasCapasYDejarSaldoCero [FAIL]
  Con error SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE02_Multicapa_DebeConsumirEnOrdenYCalcularCostosYSaldos [6 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 52.00
Actual:   50.00
  Seguimiento de la pila:
     at SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE02_Multicapa_DebeConsumirEnOrdenYCalcularCostosYSaldos() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/UnitTests/CalculadoraPepsTests.cs:line 164
  Con error SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE03_ConDecimalesYAgotamiento_DebeConsumirCapasYDejarSaldoCero [2 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 10.0000
Actual:   0
  Seguimiento de la pila:
     at SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE03_ConDecimalesYAgotamiento_DebeConsumirCapasYDejarSaldoCero() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/UnitTests/CalculadoraPepsTests.cs:line 216
  Con error SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_CuandoCapaEstaAgotada_DebeOmitirlaYConsumirDeLaSiguienteDisponible [< 1 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 15.0000
Actual:   0
  Seguimiento de la pila:
     at SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_CuandoCapaEstaAgotada_DebeOmitirlaYConsumirDeLaSiguienteDisponible() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/UnitTests/CalculadoraPepsTests.cs:line 303
  Con error SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_ConCapasDeMismaFecha_DebeRespetarOrdenPorId [< 1 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 70.00
Actual:   60.00
  Seguimiento de la pila:
     at SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_ConCapasDeMismaFecha_DebeRespetarOrdenPorId() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/UnitTests/CalculadoraPepsTests.cs:line 339
  Con error SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_SalidaIgualATodaLaExistencia_DebeAgotarTodasLasCapasYDejarSaldoCero [1 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 400.00
Actual:   300.00
  Seguimiento de la pila:
     at SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_SalidaIgualATodaLaExistencia_DebeAgotarTodasLasCapasYDejarSaldoCero() in /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/UnitTests/CalculadoraPepsTests.cs:line 262

Con error! - Con error:     5, Superado:     4, Omitido:     0, Total:     9, Duración: 33 ms - SI.Tests.dll (net8.0)
```

## Resumen del fallo
- Las pruebas fallan de manera natural porque `CalculadoraPeps.CalcularSalida` actualmente sólo consume de una sola capa y no itera multicapa en orden cronológico, ni calcula el costo unitario equivalente ni valida el saldo 0.00 cuando se agota la existencia total.
