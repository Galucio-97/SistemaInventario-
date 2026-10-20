# Evidencia Fase GREEN — SI-020: Registrar Producto (CP-01, CP-02)

## Comando ejecutado
```bash
dotnet test --filter "CP=CP-01|CP=CP-02" --no-restore --verbosity normal
```

## Salida obtenida (GREEN)
```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: null) [4 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: "   ") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(codigoInvalido: "") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreExcede150Caracteres_DebeRechazarConMensaje [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoDescripcionExcede500Caracteres_DebeRechazarConMensaje [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: null) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: "   ") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(nombreInvalido: "") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoCodigoExcede50Caracteres_DebeRechazarConMensaje [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaExcede30Caracteres_DebeRechazarConMensaje [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: "") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: "   ") [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(unidadInvalida: null) [< 1 ms]
  Correctas SI.Tests.UnitTests.Validadores.ValidadorProductoTests.Validar_CuandoDatosSonValidos_DebeAceptar [< 1 ms]
  Correctas SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.RegistrarAsync_CodigoDuplicado_RechazaYConservaUnaSolaFila [1 s]
  Correctas SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.InsertarDirecto_CodigoDuplicado_LanzaDbUpdateExceptionPorIndiceUnico [24 ms]
  Correctas SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.RegistrarAsync_ProductoValido_PersisteEnBaseDeDatosConValoresPorDefecto [48 ms]
  Correctas SI.Tests.IntegrationTests.SmokeDatabaseTests.InsertarYLeerProducto_ConContextosNuevos_PersisteEnBaseDeDatosReal [26 ms]

La serie de pruebas se ejecutó correctamente.
Pruebas totales: 18
     Correcto: 18
 Tiempo total: 2.4100 Segundos
```
