# Evidencia Fase RED — Ticket SI-023

**Fecha:** 2026-10-22  
**Rama:** `feat/SI-023-Interfaz-Administracion-Productos`  
**Objetivo:** Verificar que las pruebas funcionales de los controladores/PageModels de la interfaz de administración de productos (`RF-01`, `RNF-03`, `CP-01`, `CP-02`, `CP-11`) fallan de forma controlada y esperada debido a la ausencia de implementación en los métodos de `IndexModel`, `CrearModel` y `EditarModel`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~ProductosPagesTests"
```

---

## 2. Salida de Ejecución

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

[FAIL] IndexModel_OnGetAsync_CargaListadoCompletoConEstadoYExistencia
[FAIL] IndexModel_OnPostCambiarEstadoAsync_AlternaEstadoYRedirige
[FAIL] CrearModel_OnPostAsync_ProductoValido_RegistraYRedirigeAIndex
[FAIL] CrearModel_OnPostAsync_CodigoDuplicado_AgregaErrorDeDominioYPermaneceEnPagina
[FAIL] CrearModel_OnPostAsync_DatosInvalidos_AgregaErrorDeValidacionYPermaneceEnPagina
[FAIL] EditarModel_OnGetAsync_ProductoExistente_CargaDatosEnFormulario
[FAIL] EditarModel_OnGetAsync_ProductoInexistente_RedirigeAIndexConMensajeError
[FAIL] EditarModel_OnPostAsync_DatosValidos_ModificaProductoYRedirige
[FAIL] EditarModel_OnPostAsync_DatosInvalidos_AgregaErrorYPermaneceEnPagina

Con error! - Con error: 9, Superado: 0, Omitido: 0, Total: 9, Duración: 106 ms - SI.Tests.dll (net8.0)
```

---

## 3. Conclusión de la Fase RED

Las 9 pruebas funcionales fallan por la lógica no implementada en los stubs de Razor Pages. Se comprueba que:
- La carga del catálogo no materializa datos en el stub.
- Las acciones POST no invocan a `ProductoService` ni devuelven redirecciones.
- Las validaciones y errores de clave duplicada no se registran en `ModelState`.

El entorno está preparado para implementar la lógica de las páginas y sus vistas `.cshtml` en la fase GREEN.
