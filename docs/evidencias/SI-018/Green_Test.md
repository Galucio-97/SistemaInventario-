# Evidencia Fase GREEN — SI-018: PEPS: Salida Multicapa y Agotamiento (CP-15, CP-16, CP-17)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~CalculadoraPepsTests" --no-restore
```

## Salida obtenida (GREEN)
```text
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     9, Omitido:     0, Total:     9, Duración: 16 ms - SI.Tests.dll (net8.0)
```

## Suite completa
```bash
dotnet test
# Correctas! - Con error: 0, Superado: 59, Omitido: 0, Total: 59, Duración: 1 s - SI.Tests.dll (net8.0)
```

## Casos de Prueba Verificados
- `CalcularSalida_EscenarioE02_Multicapa_DebeConsumirEnOrdenYCalcularCostosYSaldos` (**CP-15**): Valida el consumo sucesivo en orden cronológico a través de múltiples capas (3 unidades de la primera capa @ 10.00 y 2 de la segunda @ 11.00 $\rightarrow$ Costo S1 = 52.00, Saldo = 22.00, Costo equivalente = 10.4000; Salida S2 agota existencias con saldo 0.00).
- `CalcularSalida_EscenarioE03_ConDecimalesYAgotamiento_DebeConsumirCapasYDejarSaldoCero` (**CP-17**): Valida consumos con precisión decimal y agotamiento total de capas garantizando saldo patrimonial en 0.00 (D-03 / RNF-01).
- `CalcularSalida_SalidaIgualATodaLaExistencia_DebeAgotarTodasLasCapasYDejarSaldoCero` (**CP-16**): Valida que una salida que retira exactamente todo el stock disponible consume todas las capas y deja existencia y saldo en 0.
- `CalcularSalida_CuandoCapaEstaAgotada_DebeOmitirlaYConsumirDeLaSiguienteDisponible` (**CP-15**): Asegura que capas con existencia disponible en 0 son omitidas en consumos futuros.
- `CalcularSalida_ConCapasDeMismaFecha_DebeRespetarOrdenPorId` (**CP-15**): Garantiza consistencia y determinismo en el desempate por Id cuando varias capas comparten la misma fecha de entrada.
