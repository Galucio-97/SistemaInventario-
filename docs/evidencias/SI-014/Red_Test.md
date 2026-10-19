# Evidencia Fase RED — SI-014: Validación de Existencia Suficiente (CP-07, CP-16)

## Comando ejecutado
```bash
dotnet test --filter "RF=RB-03" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.15]     SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 15.0001, existenciaDisponible: 15) [FAIL]
[xUnit.net 00:00:00.15]     SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 1, existenciaDisponible: 0) [FAIL]
[xUnit.net 00:00:00.15]     SI.Tests.UnitTests.Validadores.ValidadorExistenciaTests.Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(cantidadSalida: 20, existenciaDisponible: 15) [FAIL]

Con error! - Con error:     3, Superado:     3, Omitido:     0, Total:     6, Duración: 8 ms - SI.Tests.dll (net8.0)
```
