# Evidencia Fase RED — SI-020: Registrar Producto (CP-01, CP-02)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-01|CP=CP-02" --no-restore
```

## Salida obtenida (RED)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:00.39]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: null) [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: "   ") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: "") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreExcede150Caracteres_DebeRechazarConMensaje [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoDescripcionExcede500Caracteres_DebeRechazarConMensaje [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: null) [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: "   ") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: "") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoExcede50Caracteres_DebeRechazarConMensaje [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaExcede30Caracteres_DebeRechazarConMensaje [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: "") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: "   ") [FAIL]
[xUnit.net 00:00:00.41]     SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: null) [FAIL]
[xUnit.net 00:00:02.00]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.RegistrarAsync_CodigoDuplicado_RechazaYConservaUnaSolaFila [FAIL]
[xUnit.net 00:00:02.47]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.RegistrarAsync_ProductoValido_PersisteEnBaseDeDatosConValoresPorDefecto [FAIL]

Con error! - Con error:    15, Superado:     3, Omitido:     0, Total:    18, Duración: 2 s - SI.Tests.dll (net8.0)
```
