# Evidencia Fase RED — Ticket SI-021

**Fecha:** 2026-10-20  
**Rama:** `feat/SI-021-Consultar-Listar-Productos`  
**Objetivo:** Verificar que las pruebas de integración asociadas al caso `CP-11` y los requisitos `RF-01`, `RF-05` y `RB-04` fallan de manera controlada y esperada debido a la ausencia de implementación de `ObtenerPorIdAsync`, `ObtenerPorCodigoAsync` y `ListarTodosAsync`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~ProductoServiceIntegrationTests" --no-restore
```

---

## 2. Salida de Ejecución

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:03.80]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoExiste_RetornaProductoConSusDatos [FAIL]
  Con error SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoExiste_RetornaProductoConSusDatos [2 s]
  Mensaje de error:
   Assert.True() Failure
Expected: True
Actual:   False
  Seguimiento de la pila:
     at SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoExiste_RetornaProductoConSusDatos() in C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\IntegrationTests\ProductoServiceIntegrationTests.cs:line 129
--- End of stack trace from previous location ---
[xUnit.net 00:00:04.01]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ListarTodosAsync_RetornaTodosLosProductosConEstadoYExistencia [FAIL]
[xUnit.net 00:00:04.02]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoExiste_RetornaProductoConSusDatos [FAIL]
[xUnit.net 00:00:04.17]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje [FAIL]
[xUnit.net 00:00:04.18]     SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje [FAIL]
  Con error SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ListarTodosAsync_RetornaTodosLosProductosConEstadoYExistencia [25 ms]
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 2
Actual:   0
  Seguimiento de la pila:
     at SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ListarTodosAsync_RetornaTodosLosProductosConEstadoYExistencia() in C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\IntegrationTests\ProductoServiceIntegrationTests.cs:line 232
--- End of stack trace from previous location ---
  Con error SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoExiste_RetornaProductoConSusDatos [13 ms]
  Mensaje de error:
   Assert.True() Failure
Expected: True
Actual:   False
  Seguimiento de la pila:
     at SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoExiste_RetornaProductoConSusDatos() in C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\IntegrationTests\ProductoServiceIntegrationTests.cs:line 160
--- End of stack trace from previous location ---
  Con error SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje [7 ms]
  Mensaje de error:
   Assert.Equal() Failure: Strings differ
           ↓ (pos 0)
Expected: "El producto no existe"
Actual:   "No implementado"
           ↑ (pos 0)
  Seguimiento de la pila:
     at SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorCodigoAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje() in C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\IntegrationTests\ProductoServiceIntegrationTests.cs:line 208
--- End of stack trace from previous location ---
  Con error SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje [5 ms]
  Mensaje de error:
   Assert.Equal() Failure: Strings differ
           ↓ (pos 0)
Expected: "El producto no existe"
Actual:   "No implementado"
           ↑ (pos 0)
  Seguimiento de la pila:
     at SI.Tests.IntegrationTests.ProductoServiceIntegrationTests.ObtenerPorIdAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje() in C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\IntegrationTests\ProductoServiceIntegrationTests.cs:line 186
--- End of stack trace from previous location ---

Con error! - Con error:     5, Superado:     4, Omitido:     0, Total:     9, Duración: 377 ms - SI.Tests.dll (net8.0)
```

---

## 3. Conclusión de la Fase RED

Las 5 pruebas representativas fallan por las razones exactas de negocio (stubs retornando `"No implementado"` y listas vacías). La prueba de no alteración / idempotencia (`Consultar_NoModificaDatosNiCreaMovimientos`) pasa correctamente, validando que el entorno está listo para implementar el código de producción en la fase GREEN.
