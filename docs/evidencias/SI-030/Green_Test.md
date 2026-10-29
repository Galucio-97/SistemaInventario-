# Evidencia Fase GREEN — Ticket SI-030

**Fecha:** 2026-10-27  
**Rama:** `feat/SI-030-Interfaz-Entrada-Salidas`  
**Objetivo:** Verificar que la interfaz web de registro de movimientos (`Pages/Movimientos/Entrada` y `Pages/Movimientos/Salida`) y su integración con `InventarioService` superan al 100% las pruebas funcionales asociadas a los requisitos `RF-02`, `RF-03`, `RNF-03`, `CP-03` a `CP-07`, `CP-18`.

---

## 1. Comando Ejecutado

```powershell
$env:ConnectionStrings__TestConnection = "Server=localhost\SQLEXPRESS;Database=SistemaInventarioDb_Tests;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"; dotnet test --filter "FullyQualifiedName~MovimientosPagesTests"
```

---

## 2. Salida de Ejecución

```text
Serie de pruebas para C:\Users\gaalz\OneDrive\Documents\GitHub\SistemaInventario\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     9, Omitido:     0, Total:     9, Duración: 320 ms - SI.Tests.dll (net8.0)
```

### Ejecución de Suite Completa (Regresión General)

```text
Correctas! - Con error:     0, Superado:   126, Omitido:     0, Total:   126, Duración: 2 s - SI.Tests.dll (net8.0)
```

---

## 3. Pruebas Funcionales Validadas

| Prueba | Caso / Requisitos | Comportamiento Comprobado | Estado |
|---|---|---|---|
| `EntradaModel_OnGetAsync_CargaSoloProductosActivos` | `CP-18`, `RF-02`, `RB-10` | El combo de selección filtra estrictamente y carga únicamente productos en estado `Activo == true`. | PASÓ |
| `EntradaModel_OnPostAsync_EntradaValida_RegistraYCalculaNuevaExistencia` | `CP-03`, `RF-02`, `RNF-02` | Registra atómicamente la entrada y calcula en pantalla la nueva existencia física y valor resultante. | PASÓ |
| `EntradaModel_OnPostAsync_CantidadMenorOIgualACero_RechazaYConservaDatos` | `CP-05`, `RF-02`, `RB-01`, `RNF-03` | Rechaza cantidades ≤ 0, muestra *"La cantidad debe ser mayor que cero."* y preserva los valores capturados en el formulario. | PASÓ |
| `EntradaModel_OnPostAsync_CostoMenorOIgualACero_RechazaYConservaDatos` | `CP-06`, `RF-02`, `RB-02`, `RNF-03` | Rechaza costo unitario ≤ 0, muestra *"El costo unitario debe ser mayor que cero."* y preserva los datos del formulario. | PASÓ |
| `EntradaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos` | `CP-18`, `RF-02`, `RB-10`, `RNF-03` | Rechaza entradas a productos inactivos con *"El producto está inactivo"*. | PASÓ |
| `SalidaModel_OnGetAsync_CargaProductosConExistencia` | `RF-03`, `RF-05` | Carga el listado de productos activos mostrando la existencia física disponible en el selector. | PASÓ |
| `SalidaModel_OnPostAsync_SalidaValida_RegistraYMuestraCostosPorAmbosMetodos` | `CP-04`, `RF-03`, `RF-04` | Registra la salida descontando stock y presenta una tabla comparativa con el costo total y unitario por **PEPS** y por **Promedio Ponderado**. | PASÓ |
| `SalidaModel_OnPostAsync_CantidadExcedeExistencia_RechazaYConservaDatos` | `CP-07`, `RF-03`, `RB-03`, `RNF-03` | Rechaza salidas que superen la existencia con *"Existencia insuficiente: disponible X."* y mantiene los datos capturados. | PASÓ |
| `SalidaModel_OnPostAsync_ProductoInactivo_RechazaYConservaDatos` | `CP-18`, `RF-03`, `RB-10`, `RNF-03` | Rechaza salidas sobre productos inactivos con *"El producto está inactivo"*. | PASÓ |

---

## 4. Conclusión

La interfaz implementa los 4 pilares:
1. **Seguridad:** Filtrado exclusivo de productos activos (RB-10), validaciones rigurosas en el backend y retención segura de datos.
2. **Eficiencia:** Lectura con `.AsNoTracking()` para llenar los selectores y ejecución transaccional única en `InventarioService`.
3. **Mantenibilidad:** Separación nítida de responsabilidades en `Pages/Movimientos/` y contratos tipados.
4. **Testeabilidad:** 9 pruebas funcionales completadas en 320 ms sin requerir servidor HTTP externo.
