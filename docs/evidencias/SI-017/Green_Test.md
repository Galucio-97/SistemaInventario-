# Evidencia Fase GREEN — SI-017: PEPS: Creación de Capas y Salida de una Capa (CP-08)

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

Correctas! - Con error:     0, Superado:     4, Omitido:     0, Total:     4, Duración: 13 ms - SI.Tests.dll (net8.0)
```

## Casos de Prueba Verificados
- `CrearCapa_ConDatosEntrada_DebeInicializarCantidadDisponibleIgualAInicial`: Verifica que una entrada crea una capa asignando `CantidadDisponible = CantidadInicial`.
- `CalcularValorInventario_ConMultiplesCapas_DebeCalcularSumaDeValoresRedondeadaComoImporte`: Verifica que el valor del inventario sea la sumatoria de `(CantidadDisponible * CostoUnitario)` redondeada como importe bancario.
- `CalcularSalida_EscenarioE01_S1_DebeConsumirSoloDeCapaMasAntiguaYCalcularCostosYSaldos`: Verifica el escenario E-01 (S1) de 5 unidades consumidas exclusivamente de la capa de 10 unidades @ 20.00, resultando en costo salida 100.00 y saldo restante 400.00.
- `CalcularSalida_CuandoSalidaAgotaExactamenteCapaMasAntigua_ConsumeTodaLaCapaSinTocarLaSiguiente`: Verifica el consumo exacto de una capa completa sin afectar la subsecuente.
