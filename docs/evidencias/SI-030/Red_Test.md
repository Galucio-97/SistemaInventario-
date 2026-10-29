# Evidencia Fase RED — Ticket SI-030

**Fecha:** 2026-10-27  
**Rama:** `feat/SI-030-Interfaz-Entrada-Salidas`  
**Objetivo:** Verificar que las pruebas funcionales para los formularios y controladores de registro de entradas y salidas (`RF-02`, `RF-03`, `RNF-03`, `CP-03` a `CP-07`, `CP-18`) fallan de forma esperada y controlada debido a la ausencia de implementación en los PageModels `EntradaModel` y `SalidaModel`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~MovimientosPagesTests"
```

---

## 2. Salida de Ejecución

```text
Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

[FAIL] EntradaModel_OnGetAsync_CargaSoloProductosActivos
[FAIL] EntradaModel_OnPostAsync_EntradaValida_RegistraYCalculaNuevaExistencia
[FAIL] EntradaModel_OnPostAsync_CantidadMenorOIgualACero_RechazaYConservaDatos
[FAIL] EntradaModel_OnPostAsync_CostoMenorOIgualACero_RechazaYConservaDatos
[FAIL] EntradaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos
[FAIL] SalidaModel_OnGetAsync_CargaProductosConExistencia
[FAIL] SalidaModel_OnPostAsync_SalidaValida_RegistraYMuestraCostosPorAmbosMetodos
[FAIL] SalidaModel_OnPostAsync_CantidadExcedeExistencia_RechazaYConservaDatos
[FAIL] SalidaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos

Con error! - Con error: 9, Superado: 0, Omitido: 0, Total: 9, Duración: 633 ms - SI.Tests.dll (net8.0)
```

---

## 3. Conclusión de la Fase RED

Las 9 pruebas funcionales fallan porque `EntradaModel` y `SalidaModel` devuelven colecciones vacías o `Page()` sin invocar a `InventarioService` ni gestionar `ModelState`. El entorno está listo para proceder a la fase GREEN implementando la lógica en los PageModels y creando las vistas Razor `.cshtml`.
