# Evidencia Fase RED — Ticket SI-022

**Fecha:** 2026-10-21  
**Rama:** `feat/SI-022-Modificar-Producto`  
**Objetivo:** Verificar que las pruebas unitarias y de integración para la modificación de productos y cambio de estado (`RF-01`, `RB-10`, `RNF-03`, `CP-18`) fallan de forma controlada y esperada debido a los stubs que retornan `"No implementado"`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~ValidadorProductoTests|FullyQualifiedName~ProductoServiceIntegrationTests"
```

---

## 2. Salida de Ejecución

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje (null, "", "   ")
[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoNombreEsVacio_DebeRechazarConMensaje (null, "", "   ")
[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoNombreExcede150Caracteres_DebeRechazarConMensaje
[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoUnidadMedidaExcede30Caracteres_DebeRechazarConMensaje
[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoDescripcionExcede500Caracteres_DebeRechazarConMensaje
[FAIL] ValidadorProductoTests.ValidarModificacion_CuandoDatosSonValidos_DebeAceptar

[FAIL] ProductoServiceIntegrationTests.ModificarAsync_ProductoExistente_ActualizaCamposPermitidosYNoModificaCodigoNiExistenciasNiValores
[FAIL] ProductoServiceIntegrationTests.ModificarAsync_ProductoInexistente_RetornaFalsoConMensajeNoExiste
[FAIL] ProductoServiceIntegrationTests.ModificarAsync_DatosInvalidos_RetornaFalsoConMensajeValidacion
[FAIL] ProductoServiceIntegrationTests.CambiarEstadoAsync_ProductoExistente_CambiaActivoSinModificarOtrosCampos
[FAIL] ProductoServiceIntegrationTests.CambiarEstadoAsync_ProductoInexistente_RetornaFalsoConMensajeNoExiste

Con error! - Con error: 15, Superado: 23, Omitido: 0, Total: 38, Duración: 1 s - SI.Tests.dll (net8.0)
```

---

## 3. Conclusión de la Fase RED

15 casos de prueba fallan debido a la ausencia de lógica en los stubs (`ValidadorProducto.ValidarModificacion`, `ProductoService.ModificarAsync` y `ProductoService.CambiarEstadoAsync`), mientras que las pruebas existentes de registro y consulta continúan pasando de manera íntegra. El entorno está listo para avanzar a la fase GREEN.
