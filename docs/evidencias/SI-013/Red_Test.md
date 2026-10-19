# Evidencia Fase RED — SI-013: Validación de Costo Unitario (CP-06)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-06" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:02.09]     SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 10.12345) [FAIL]
[xUnit.net 00:00:02.10]     SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 1.00001) [FAIL]
[xUnit.net 00:00:02.10]     SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 0.00001) [FAIL]
[xUnit.net 00:00:02.10]     SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(costoInvalido: -0.01) [FAIL]
[xUnit.net 00:00:02.11]     SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(costoInvalido: 0) [FAIL]

Con error! - Con error:     5, Superado:     4, Omitido:     0, Total:     9, Duración: 37 ms - SI.Tests.dll (net8.0)
```
