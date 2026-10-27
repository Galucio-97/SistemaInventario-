# Evidencia Fase GREEN — SI-025: Registrar Salida con Transacción (CP-04, CP-05, CP-07, CP-11, CP-15, CP-16, CP-18)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~RegistrarSalidaAsync"
```

## Salida obtenida (GREEN)
```text
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    10, Omitido:     0, Total:    10, Duración: 160 ms - SI.Tests.dll (net8.0)
```

## Suite completa del proyecto
```bash
dotnet test
# Correctas! - Con error: 0, Superado: 117, Omitido: 0, Total: 117, Duración: 1 s - SI.Tests.dll (net8.0)
```

## Casos de Prueba Verificados
- `RegistrarSalidaAsync_SalidaValidaEscenarioE01_S1_ConsumeCapasCalculaCostosAmbosMetodosYActualizaProductoEnTransaccion` (**CP-04**): Salida S1 de E-01 (5 unidades sobre E1 de 10 @ 20.00 y E2 de 10 @ 30.00) $\rightarrow$ existencia restante 15, costo PEPS 100.00 / saldo 400.00, costo Promedio 125.00 / saldo 375.00, capa 1 con 5 disponibles y capa 2 con 10 disponibles.
- `RegistrarSalidaAsync_SalidaIgualATodaLaExistencia_AceptaDejaExistenciaCeroYValoresEnCero` (**CP-16**): Salida que retira el 100% del inventario restante $\rightarrow$ aceptada, existencia 0.0000, saldos PEPS y Promedio en 0.00 y todas las capas agotadas.
- `RegistrarSalidaAsync_CuandoExistenciaEsInsuficiente_RechazaYNoModificaBaseDeDatos` (**CP-07**): Casos que exceden la existencia física $\rightarrow$ rechazados con mensaje `"Existencia insuficiente: disponible X."` garantizando que la base de datos no sufre cambios.
- `RegistrarSalidaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos` (**CP-05**): Rechazo de cantidades $\le 0$ o $> 4$ decimales.
- `RegistrarSalidaAsync_CuandoProductoNoExiste_RechazaConMensajeYNoModificaBaseDeDatos` (**CP-11**): Rechazo ante producto inexistente (`"El producto no existe"`).
- `RegistrarSalidaAsync_CuandoProductoEstaInactivo_RechazaConMensajeYNoModificaBaseDeDatos` (**CP-18**): Rechazo ante producto inactivo (`"El producto está inactivo"`).
- `RegistrarSalidaAsync_EscenarioE02_SalidaMulticapa_ConsumeCapasEnOrdenYCalculaCostos` (**CP-15**): Salida multicapa en Escenario E-02 consumiendo capas en estricto orden cronológico (`FechaEntrada ASC, Id ASC`).
