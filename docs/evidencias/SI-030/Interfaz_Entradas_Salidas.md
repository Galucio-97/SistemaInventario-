# Evidencia Técnica — SI-030: Interfaz de Registro de Entradas y Salidas

## 1. Resumen del Ticket
- **ID:** `SI-030`
- **Release:** `v0.4.0` (Movimientos transaccionales)
- **Responsable:** Dev B
- **Objetivo:** Registrar movimientos de entrada y salida desde la interfaz web, observar la actualización atómica de existencias y costos en ambos métodos (PEPS y Promedio), y verificar la retención de datos ante rechazos (**RF-02**, **RF-03**, **RNF-03**, **RB-01..03**, **RB-10**, **CP-03 a CP-07**, **CP-18**).
- **Trazabilidad:** `RF-02` (Entradas), `RF-03` (Salidas), `RF-04` (Métodos de costeo), `RNF-03` (Mensajes claros de error), `RB-10` (Rechazo en producto inactivo).

---

## 2. Criterios de Aceptación Verificados

### 2.1 Formulario de Entrada (`Pages/Movimientos/Entrada.cshtml`)
- Selector de productos filtrado por `Activo == true` (cumple estrictamente `RB-10`).
- Campos: Producto, Fecha (`type="date"`), Cantidad (`step="0.0001"`), Costo Unitario (`step="0.0001"`).
- Tras el registro exitoso: Muestra tarjeta informativa con la cantidad ingresada, la existencia previa, la nueva existencia física y el valor total del movimiento.

### 2.2 Formulario de Salida (`Pages/Movimientos/Salida.cshtml`)
- Selector de productos que visualiza la existencia disponible en tiempo real: `[P-001] Tornillo — Existencia disponible: 10.0000 pza`.
- Campos: Producto, Fecha, Cantidad a despachar.
- Tras el registro exitoso: Muestra la nueva existencia física y una tabla comparativa con:
  - **PEPS:** Costo total de la salida y Costo unitario equivalente consumido de las capas.
  - **Costo Promedio:** Costo total de la salida y Costo promedio ponderado aplicado.

### 2.3 Manejo de Rechazos y Conservación de Datos
- **Cantidad ≤ 0 (CP-05, RB-01):** *"La cantidad debe ser mayor que cero."*
- **Costo Unitario ≤ 0 (CP-06, RB-02):** *"El costo unitario debe ser mayor que cero."*
- **Salida > Existencia (CP-07, RB-03):** *"Existencia insuficiente: disponible {existencia}."*
- **Producto Inactivo (CP-18, RB-10):** *"El producto está inactivo"*
- En todos los rechazos, el formulario conserva íntegros los datos que el usuario capturó (`Input.Cantidad`, `Input.CostoUnitario`, etc.) para facilitar su corrección inmediata sin tener que reescribir todo el formulario.

---

## 3. Implementación y Arquitectura

### Controlador de Entrada (`Pages/Movimientos/Entrada.cshtml.cs`)
Invoca de forma atómica a `IInventarioService.RegistrarEntradaAsync`, manejando el estado del formulario ante errores o éxitos.

### Controlador de Salida (`Pages/Movimientos/Salida.cshtml.cs`)
Invoca `IInventarioService.RegistrarSalidaAsync`, consultando las capas PEPS y el costo promedio vigente para poblar `ResultadoSalidaDto`.

---

## 4. Resumen de Pruebas Automatizadas

```text
Pruebas funcionales de Razor Pages (SI.Tests.FunctionalTests.MovimientosPagesTests):
- 9 de 9 pruebas superadas (100%). Duración: 320 ms.

Suite completa de la solución:
- 126 de 126 pruebas superadas (0 errores). Duración: 2 s.
```
