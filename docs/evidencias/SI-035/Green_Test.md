# Evidencia de Fase GREEN — SI-035: Vista comparativa PEPS vs Promedio

- **Ticket:** SI-035
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-035-Vista-Comparativa-Peps-Promedio`
- **Requisitos cubiertos:** RF-04, CP-10

## 1. Pruebas Ejecutadas (`VistaComparativaPageTests.cs`)

1. `ConsultaModel_OnGetAsync_CargaHistorialComparativoParalelo_ConCantidadesFisicasIdenticas` (CP-10, RF-04, SI-035) -> **PASÓ**
   - Verifica que `HistorialComparativo` tenga las 3 filas del Escenario E-01.
   - Verifica que las cantidades físicas y existencias resultantes sean **estrictamente idénticas** en ambas columnas.
   - Verifica que en la Salida 1 (5 unidades):
     - Costo Total PEPS: $100.00 | Saldo Valor PEPS: $400.00
     - Costo Total Promedio: $125.00 | Saldo Valor Promedio: $375.00
     - Variación de Costo Total: -$25.00 (PEPS reporta menor costo)
     - Variación de Saldo Valor: +$25.00 (PEPS conserva mayor valor de inventario)

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
Correctas! - Con error: 0, Superado: 166, Omitido: 0, Total: 166, Duración: 6 s
Suite completa del sistema: 166 / 166 pruebas superadas (100% de éxito, 0 errores).
```

Estado: **VERDE CONFIRMADO**
