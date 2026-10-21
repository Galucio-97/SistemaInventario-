# Evidencia Fase RED — SI-016: Promedio Ponderado: Salidas y Ajuste por Residuo (CP-09, CP-17)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~CalcularSalida" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.45]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE03_SalidaS1_AplicaRedondeoBancarioParImpar [FAIL]
[xUnit.net 00:00:00.46]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE02_SalidaS1_CalculaCostoConCuatroDecimalesYRedondeoImporte [FAIL]
[xUnit.net 00:00:00.46]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE02_SalidaS2_AgotamientoTotal_AjustaResiduoYDejaSaldoEnCero [FAIL]
[xUnit.net 00:00:00.46]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE01_SalidaParcial_MantieneCostoPromedioYDescuentaSaldo [FAIL]
[xUnit.net 00:00:00.46]     SI.Tests.UnitTests.CalculadoraPromedioPonderadoTests.CalcularSalida_EscenarioE03_SalidaS2_AgotamientoTotal_AjustaCentavoResiduoD03 [FAIL]

Con error! - Con error:     5, Superado:     0, Omitido:     0, Total:     5, Duración: 7 ms - SI.Tests.dll (net8.0)
```
