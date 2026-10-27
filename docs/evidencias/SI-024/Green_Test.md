# Evidencia Fase GREEN — SI-024: Registrar Entrada con Transacción (CP-03, CP-05, CP-06, CP-11, CP-18)

## Comando ejecutado
```bash
dotnet test --filter "FullyQualifiedName~InventarioServiceIntegrationTests"
```

## Salida obtenida (GREEN)
```text
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    10, Omitido:     0, Total:    10, Duración: 238 ms - SI.Tests.dll (net8.0)
```

## Suite completa
```bash
dotnet test
# Correctas! - Con error: 0, Superado: 107, Omitido: 0, Total: 107, Duración: 849 ms - SI.Tests.dll (net8.0)
```

## Casos de Prueba Verificados
- `RegistrarEntradaAsync_EntradaValida_CreaMovimientoCapaYActualizaProductoEnTransaccion` (**CP-03**, `RF-02`, `RB-04`, `RB-05`, `RNF-04`):
  - Verifica el registro atómico de una entrada válida (E-01 E1: 10 @ 20.00).
  - Valida actualización de `Producto` en SQL Server: existencia 10, promedio 20.0000, valor promedio 200.00, valor PEPS 200.00.
  - Valida inserción de fila en `Movimientos` con tipo `Entrada` y saldos resultantes.
  - Valida inserción de fila en `CapasInventarioPeps` vinculada al movimiento y producto.
- `RegistrarEntradaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos` (**CP-05**, `RB-01`, `RNF-04`):
  - Verifica rechazo con mensajes descriptivos ante cantidades negativas, cero y con más de 4 decimales.
  - Comprueba que la base de datos no sufre cambios (0 movimientos, 0 capas).
- `RegistrarEntradaAsync_CuandoCostoUnitarioEsInvalido_RechazaYNoModificaBaseDeDatos` (**CP-06**, `RB-02`, `RNF-04`):
  - Verifica rechazo ante costos unitarios negativos, cero o con más de 4 decimales sin modificar la base de datos.
- `RegistrarEntradaAsync_CuandoProductoNoExiste_RechazaConMensajeYNoModificaBaseDeDatos` (**CP-11**, `RB-04`):
  - Verifica rechazo con `"El producto no existe"` sin insertar movimientos ni capas.
- `RegistrarEntradaAsync_CuandoProductoEstaInactivo_RechazaConMensajeYNoModificaBaseDeDatos` (**CP-18**, `RB-10`):
  - Verifica que operaciones sobre productos inactivos son rechazadas con `"El producto está inactivo"` preservando la invariabilidad de la base de datos.
- `RegistrarEntradaAsync_MultiplesEntradasConsecutivas_AcumulaExistenciasCapasYActualizaPromedioPonderado` (`RF-02`, `RB-04`, `RB-05`):
  - Simula entradas consecutivas E1 (10 @ 20.00) y E2 (10 @ 30.00) de E-01, verificando acumulación a 20 unidades, promedio 25.0000, saldos 500.00 y 2 capas independientes.
