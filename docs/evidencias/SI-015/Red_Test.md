# Evidencia Fase RED — SI-015: Promedio Ponderado: Entradas (CP-09)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-09" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.73]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE03_SegundaEntrada_AplicaRedondeoBancarioParImpar [FAIL]
[xUnit.net 00:00:00.74]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE01_PrimeraEntrada_CalculaPromedioYSaldoCorrectos [FAIL]
[xUnit.net 00:00:00.74]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE02_SegundaEntrada_AplicaRedondeoBancarioACuatroDecimales [FAIL]
[xUnit.net 00:00:00.74]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE01_SegundaEntrada_RecalculaPromedioPonderadoYSaldo [FAIL]

Con error! - Con error:     4, Superado:     1, Omitido:     0, Total:     5, Duración: 13 ms - SI.Tests.dll (net8.0)
```
