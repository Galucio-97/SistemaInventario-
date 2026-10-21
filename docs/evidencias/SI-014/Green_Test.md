# Evidencia Fase GREEN — SI-014: Validación de Existencia Suficiente (CP-07, CP-16)

## Comando ejecutado
```bash
dotnet test --filter "RF=RB-03" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMenorOIgualAExistencia_DebeAceptar(cantidadSalida: 0.0001, existenciaDisponible: 10) [6 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMenorOIgualAExistencia_DebeAceptar(cantidadSalida: 14.9999, existenciaDisponible: 15) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMenorOIgualAExistencia_DebeAceptar(cantidadSalida: 15, existenciaDisponible: 15) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 15.0001, existenciaDisponible: 15) [6 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 1, existenciaDisponible: 0) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 20, existenciaDisponible: 15) [< 1 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 6
     Correcto: 6
 Tiempo total: 0.6517 Segundos
```
