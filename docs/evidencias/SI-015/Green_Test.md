# Evidencia Fase GREEN — SI-015: Promedio Ponderado: Entradas (CP-09)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-09" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE03_SegundaEntrada_AplicaRedondeoBancarioParImpar [170 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_ConCantidadTotalCero_NoLanzaDivisionPorCero [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE01_PrimeraEntrada_CalculaPromedioYSaldoCorrectos [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE02_SegundaEntrada_AplicaRedondeoBancarioACuatroDecimales [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularEntrada_EscenarioE01_SegundaEntrada_RecalculaPromedioPonderadoYSaldo [< 1 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 5
     Correcto: 5
 Tiempo total: 2.0362 Segundos
```
