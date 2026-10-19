# Evidencia Fase RED — SI-012: Validación de Cantidades (CP-05)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-05" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.14]     SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: -1) [FAIL]
[xUnit.net 00:00:00.14]     SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: 0) [FAIL]
[xUnit.net 00:00:00.14]     SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 1.00001) [FAIL]
[xUnit.net 00:00:00.14]     SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 0.00001) [FAIL]
[xUnit.net 00:00:00.14]     SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 10.12345) [FAIL]

Con error! - Con error:     5, Superado:     4, Omitido:     0, Total:     9, Duración: 8 ms - SI.Tests.dll (net8.0)
```
