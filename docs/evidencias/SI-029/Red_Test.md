# Evidencia Fase RED — SI-029: Rechazo de fechas retroactivas (RB-11, D-04, CP-20)

## Objetivo de la fase RED
Diseñar y formular la suite de pruebas unitarias y de integración para dar cumplimiento estricto a `CP-20` (`RB-11`, `D-04`), garantizando que cualquier movimiento con fecha anterior al último movimiento registrado para el producto sea rechazado sin impacto en base de datos, mientras que fechas iguales o posteriores sean admitidas.

## Casos de prueba diseñados

### 1. Pruebas Unitarias (`ValidadorFechaMovimientoTests` - CP-20, RB-11)
- `Validar_CuandoFechaEsPosteriorAUltimoMovimiento_DebeAceptar`: Fecha posterior al último movimiento es válida.
- `Validar_CuandoFechaEsIgualAUltimoMovimiento_DebeAceptar`: Fecha idéntica al último movimiento es válida.
- `Validar_CuandoFechaEsAnteriorAUltimoMovimiento_DebeRechazarConMensaje`: Rechazo con mensaje *"La fecha del movimiento no puede ser anterior al último movimiento registrado."*.
- `Validar_CuandoNoExistenMovimientosPrevios_DebeAceptar`: Primer movimiento de un producto no tiene restricción temporal previa.

### 2. Pruebas de Integración (`InventarioServiceIntegrationTests` - CP-20, RB-11)
- `RegistrarEntradaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos`: Comprueba rechazo transaccional e invariabilidad en BD tras intento de entrada retroactiva.
- `RegistrarEntradaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente`: Permite entrada con misma marca temporal que el último movimiento.
- `RegistrarSalidaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos`: Comprueba rechazo transaccional e invariabilidad en BD tras intento de salida retroactiva.
- `RegistrarSalidaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente`: Permite salida con misma marca temporal que el último movimiento.

## Comando ejecutado (Fase RED)
```bash
dotnet test --filter "FullyQualifiedName~FechaMovimiento|FullyQualifiedName~CuandoFechaEs"
```

## Salida de pruebas (RED)
```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.10]     SI.Tests.UnitTests.Validadores.ValidadorFechaMovimientoTests.Validar_CuandoFechaEsPosteriorAUltimoMovimiento_DebeAceptar [FAIL]
[xUnit.net 00:00:00.10]     SI.Tests.UnitTests.Validadores.ValidadorFechaMovimientoTests.Validar_CuandoFechaEsAnteriorAUltimoMovimiento_DebeRechazarConMensaje [FAIL]
[xUnit.net 00:00:00.10]     SI.Tests.UnitTests.Validadores.ValidadorFechaMovimientoTests.Validar_CuandoFechaEsIgualAUltimoMovimiento_DebeAceptar [FAIL]
[xUnit.net 00:00:00.10]     SI.Tests.UnitTests.Validadores.ValidadorFechaMovimientoTests.Validar_CuandoNoExistenMovimientosPrevios_DebeAceptar [FAIL]
[xUnit.net 00:00:00.67]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos [FAIL]
[xUnit.net 00:00:00.68]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos [FAIL]
[xUnit.net 00:00:00.69]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarEntradaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente [FAIL]
[xUnit.net 00:00:00.71]     SI.Tests.IntegrationTests.InventarioServiceIntegrationTests.RegistrarSalidaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente [FAIL]

Con error! - Con error:     8, Superado:     0, Omitido:     0, Total:     8, Duración: 608 ms - SI.Tests.dll (net8.0)
```
