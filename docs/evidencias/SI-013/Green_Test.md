# Evidencia Fase GREEN — SI-013: Validación de Costo Unitario (CP-06)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-06" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsValido_DebeAceptar(costoValido: 0.01) [6 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsValido_DebeAceptar(costoValido: 0.0001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsValido_DebeAceptar(costoValido: 100.1234) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsValido_DebeAceptar(costoValido: 20) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 10.12345) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 1.00001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(costoInvalido: 0.00001) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(costoInvalido: -0.01) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorCostoUnitarioTests.ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(costoInvalido: 0) [< 1 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 9
     Correcto: 9
 Tiempo total: 1.3449 Segundos
```
