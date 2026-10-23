# Evidencia Fase GREEN — Ticket SI-023

**Fecha:** 2026-10-22  
**Rama:** `feat/SI-023-Interfaz-Administracion-Productos`  
**Objetivo:** Verificar que la interfaz web de administración de productos (`Pages/Productos/Index`, `Crear`, `Editar`) y su integración con `ProductoService` hacen pasar al 100% las pruebas funcionales asociadas a los requisitos `RF-01`, `RNF-03`, `CP-01`, `CP-02`, `CP-11`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~ProductosPagesTests"
```

---

## 2. Salida de Ejecución

```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     9, Omitido:     0, Total:     9, Duración: 184 ms - SI.Tests.dll (net8.0)
```

### Ejecución de Suite Completa (Regresión General)

```text
Correctas! - Con error:     0, Superado:   106, Omitido:     0, Total:   106, Duración: 3 s - SI.Tests.dll (net8.0)
```

---

## 3. Pruebas Funcionales Validadas

| Prueba | Caso / Requisitos | Comportamiento Comprobado | Estado |
|---|---|---|---|
| `IndexModel_OnGetAsync_CargaListadoCompletoConEstadoYExistencia` | `CP-01`, `RF-01`, `RF-05` | Recupera y muestra todos los productos con código, nombre, unidad, estado y existencia física. | PASÓ |
| `IndexModel_OnPostCambiarEstadoAsync_AlternaEstadoYRedirige` | `RF-01`, `RB-10` | Ejecuta cambio de estado (Activo/Inactivo) mediante POST seguro y actualiza en base de datos. | PASÓ |
| `CrearModel_OnPostAsync_ProductoValido_RegistraYRedirigeAIndex` | `CP-01`, `RF-01`, `RNF-02` | Procesa alta válida, persiste en base de datos y redirige con mensaje de éxito en TempData. | PASÓ |
| `CrearModel_OnPostAsync_CodigoDuplicado_AgregaErrorDeDominioYPermaneceEnPagina` | `CP-02`, `RF-01`, `RNF-03` | Captura rechazo por duplicado y presenta en pantalla el mensaje exacto del sistema sin excepción técnica. | PASÓ |
| `CrearModel_OnPostAsync_DatosInvalidos_AgregaErrorDeValidacionYPermaneceEnPagina` | `CP-02`, `RF-01`, `RNF-03` | Valida campos obligatorios y longitudes máximas a través de las reglas de dominio. | PASÓ |
| `EditarModel_OnGetAsync_ProductoExistente_CargaDatosEnFormulario` | `CP-01`, `RF-01` | Carga los datos del producto seleccionado para su edición. | PASÓ |
| `EditarModel_OnGetAsync_ProductoInexistente_RedirigeAIndexConMensajeError` | `CP-11`, `RF-01`, `RB-04` | Ante consulta de ID inexistente, redirige al listado con mensaje *"El producto no existe"*. | PASÓ |
| `EditarModel_OnPostAsync_DatosValidos_ModificaProductoYRedirige` | `RF-01`, `RB-10` | Actualiza nombre, unidad, descripción y estado, manteniendo el código inmutable (D-10) y existencias intactas. | PASÓ |
| `EditarModel_OnPostAsync_DatosInvalidos_AgregaErrorYPermaneceEnPagina` | `RF-01`, `RNF-03` | Rechaza datos inválidos en edición y muestra el mensaje correspondiente en ModelState. | PASÓ |

---

## 4. Conclusión

La interfaz de usuario cumple fielmente con el principio de que ninguna regla de negocio reside exclusivamente en el cliente: el backend (`ValidadorProducto` y `ProductoService`) gobierna todas las reglas y la UI las refleja fidedignamente.
