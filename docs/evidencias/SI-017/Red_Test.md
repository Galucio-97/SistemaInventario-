# Evidencia Fase RED — SI-017: PEPS: Creación de Capas y Salida de una Capa (CP-08)

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
[xUnit.net 00:00:00.35]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularValorInventario_ConMultiplesCapas_DebeCalcularSumaDeValoresRedondeadaComoImporte [FAIL]
[xUnit.net 00:00:00.36]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_EscenarioE01_S1_DebeConsumirSoloDeCapaMasAntiguaYCalcularCostosYSaldos [FAIL]
[xUnit.net 00:00:00.36]     SI.Tests.UnitTests.CalculadoraPepsTests.CrearCapa_ConDatosEntrada_DebeInicializarCantidadDisponibleIgualAInicial [FAIL]
[xUnit.net 00:00:00.36]     SI.Tests.UnitTests.CalculadoraPepsTests.CalcularSalida_CuandoSalidaAgotaExactamenteCapaMasAntigua_ConsumeTodaLaCapaSinTocarLaSiguiente [FAIL]

Con error! - Con error:     4, Superado:     0, Omitido:     0, Total:     4, Duración: 9 ms - SI.Tests.dll (net8.0)
```
