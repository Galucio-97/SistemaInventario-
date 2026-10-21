# Evidencia Fase GREEN — SI-012: Validación de Cantidades (CP-05)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-05" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 1) [3 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 10.5) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 100.1234) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsValida_DebeAceptar(cantidadValida: 0.0001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: -1) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(cantidadInvalida: 0) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 1.00001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 0.00001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCantidadTests.ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(cantidadInvalida: 10.12345) [< 1 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 9
     Correcto: 9
 Tiempo total: 0.6394 Segundos
```
