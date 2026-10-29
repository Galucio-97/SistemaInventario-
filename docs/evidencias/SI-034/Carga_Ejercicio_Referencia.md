# Documentación de Funcionalidad — SI-034: Carga del ejercicio de referencia desde la interfaz

## Resumen del Ticket
- **Identificador:** SI-034
- **Versión:** v0.5.0
- **Prioridad:** Could (Completado con éxito anticipadamente)
- **Requisitos:** RF-04, RF-05, CP-08, CP-09
- **Objetivo:** Demostrar el escenario E-01 en vivo con un solo clic durante la evaluación.

## Principios de Diseño MEST Aplicados
1. **Mantenible:** Se implementó `IEjercicioReferenciaService` y `EjercicioReferenciaService` desacoplando la lógica de demostración sin contaminar los servicios de dominio principales, y reutilizando directamente `ProductoService` e `InventarioService`.
2. **Eficiente:** Búsqueda rápida por código con `.AsNoTracking()`. En caso de existir, la operación toma menos de 10ms.
3. **Seguro:** Manejo atómico y transaccional mediante los servicios oficiales de inventario. Idempotencia garantizada: si el producto ya existe, no se duplica ningún registro ni movimiento.
4. **Testeable:** Cubierto con pruebas de integración y funcionales automatizadas sin mocks, ejecutadas contra la base de datos de pruebas.

## Coincidencia Fila por Fila con el Oráculo E-01
- **Producto:** `E01-DEMO` ("Tornillo 1/4 (Ejercicio E-01)")
- **Entrada 1:** 10 u @ $20.00
- **Entrada 2:** 10 u @ $30.00
- **Salida 1:** 5 u
- **Resultado PEPS:** Existencia 15 u, Valuación $400.00 MXN, Costo unitario $26.6667 MXN.
- **Resultado Promedio:** Existencia 15 u, Valuación $375.00 MXN, Costo unitario $25.0000 MXN.
- **Acción UI:** Botón verde *"Cargar Ejercicio E-01 (Demostración)"* disponible en `/Inventario/Consulta`.
