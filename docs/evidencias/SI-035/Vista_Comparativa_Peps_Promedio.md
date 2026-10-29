# Documentación de Funcionalidad — SI-035: Vista comparativa PEPS vs Promedio

## Resumen del Ticket
- **Identificador:** SI-035
- **Versión:** v0.5.0
- **Prioridad:** Could (Completado con éxito anticipadamente)
- **Requisitos:** RF-04, CP-10
- **Objetivo:** Mostrar ambos métodos lado a lado para evidenciar CP-10 en una sola captura.

## Principios MEST Aplicados
1. **Mantenible:** Se modeló el DTO inmutable `MovimientoComparativoDto` y se agregó el método `ObtenerHistorialComparativoAsync` en `IInventarioService`, separando responsabilidades y manteniendo código limpio.
2. **Eficiente:** Los movimientos se procesan con una sola consulta a EF Core usando `.AsNoTracking()` en orden cronológico (`Fecha ASC, Id ASC`).
3. **Seguro:** No produce efectos colaterales; las cantidades físicas son inmutables e idénticas en ambas columnas.
4. **Testeable:** Cubierto con pruebas funcionales automatizadas sin mocks verificando cada aserción del oráculo CP-10.

## Tabla Comparativa Paralela (Escenario E-01 / CP-10)

| # | Fecha | Tipo | Cantidad (Física) | Existencia (Física) | Costo Total PEPS | Saldo Valor PEPS | Costo Total Promedio | Saldo Valor Promedio | Δ Saldo (PEPS - Prom) |
|---|---|---|---|---|---|---|---|---|---|
| 1 | 2026-10-01 | + Entrada | 10.0000 | 10.0000 | $200.00 | $200.00 | $200.00 | $200.00 | $0.00 |
| 2 | 2026-10-02 | + Entrada | 10.0000 | 20.0000 | $300.00 | $500.00 | $300.00 | $500.00 | $0.00 |
| 3 | 2026-10-03 | - Salida | 5.0000 | 15.0000 | $100.00 | $400.00 | $125.00 | $375.00 | **+$25.00** |

## Evidencia Observable en Pantalla
- Ruta: `/Inventario/Consulta?ProductoId={id}`
- Card con borde azul y encabezado: **"Vista Comparativa en Paralelo (PEPS vs Promedio Ponderado - CP-10)"**.
- En una sola pantalla/captura se evidencian simultáneamente todos los datos exigidos por el oráculo de pruebas CP-10.
