# Evidencia Fase GREEN — SI-016: Promedio Ponderado: Salidas y Ajuste por Residuo (CP-09, CP-17)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~CalcularSalida" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_CuandoSalidaMayorAExistencia_LanzaInvalidOperationException [8 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE03_SalidaS1_AplicaRedondeoBancarioParImpar [3 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE02_SalidaS1_CalculaCostoConCuatroDecimalesYRedondeoImporte [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE02_SalidaS2_AgotamientoTotal_AjustaResiduoYDejaSaldoEnCero [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE01_SalidaParcial_MantieneCostoPromedioYDescuentaSaldo [< 1 ms]
  Correctas SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE03_SalidaS2_AgotamientoTotal_AjustaCentavoResiduoD03 [< 1 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 6
     Correcto: 6
 Tiempo total: 1.5147 Segundos
```
