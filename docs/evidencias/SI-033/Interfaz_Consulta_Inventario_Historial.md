# Documentación de Funcionalidad — SI-033: Interfaz de consulta de inventario e historial

## Resumen del Ticket
- **Identificador:** SI-033
- **Versión:** v0.5.0
- **Requisitos:** RF-04, RF-05, CP-08, CP-09, CP-10, CP-11
- **Componentes:**
  - `SI.Web/Pages/Inventario/Consulta.cshtml`: Vista interactiva con selector de producto, selector de método (PEPS / Promedio), resumen visible con tarjetas KPI, tabla comparativa dual, desglose de capas PEPS activas y tabla de historial de movimientos (Kardex).
  - `SI.Web/Pages/Inventario/Consulta.cshtml.cs`: PageModel con carga asíncrona inmutable (`AsNoTracking`) de existencias e historial de movimientos.
  - Formatos numéricos conformes a especificación: Importes y saldos con 2 decimales (`N2`), cantidades y costos unitarios con 4 decimales (`N4`).

## Comportamiento Observable de la Interfaz

### 1. Selector de Producto y Método
- Menú desplegable con los productos registrados indicando código, nombre y estado.
- Selector de método de costeo: PEPS o Promedio Ponderado.
- Botón de consulta rápida.

### 2. Resumen de Inventario y Métricas Clave
- **Existencia Física Actual:** Mostrada con 4 decimales (`N4`) y unidad de medida.
- **Costo Unitario Vigente:** Mostrado con 4 decimales (`N4`) según la regla D-09 para PEPS y el costo promedio actual para Promedio Ponderado.
- **Valuación Total del Inventario:** Mostrada con 2 decimales (`N2`) en moneda nacional ($ MXN).

### 3. Comparativa Dual Simultánea
- Muestra en paralelo los datos bajo el método PEPS y bajo el método Promedio Ponderado, con cálculo automático de la diferencia patrimonial/financiera entre ambos.

### 4. Desglose de Capas PEPS Activas
- Muestra cada lote o capa que aún conserva saldo (`CantidadDisponible > 0`), con fecha de entrada, lote original, saldo disponible, costo unitario de adquisición y subtotal valuado.

### 5. Historial de Movimientos (Kardex)
- Tabla cronológica completa:
  - Número de movimiento
  - Fecha y hora
  - Tipo de movimiento (Badge verde `+ Entrada` o rojo `- Salida`)
  - Cantidad
  - Costo Unitario asignado según método
  - Costo Total del movimiento
  - Existencia Resultante acumulada
  - Saldo de Valor acumulado del inventario
