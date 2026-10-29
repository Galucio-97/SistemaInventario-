# Evidencia de Fase GREEN — SI-034: Carga del ejercicio de referencia desde la interfaz

- **Ticket:** SI-034
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-034-Carga-Ejercicio-Referencia`
- **Requisitos cubiertos:** CP-08, CP-09, RF-04, RF-05

## 1. Pruebas Ejecutadas

### Pruebas de Integración (`EjercicioReferenciaIntegrationTests.cs`)
1. `CargarEjercicioE01Async_PrimeraVez_CreaProductoYMovimientosE01ExactosConOraculo` (CP-08, CP-09) -> **PASÓ**
   - Crea producto `E01-DEMO` ("Tornillo 1/4 (Ejercicio E-01)").
   - Registra Entrada 1: 10 @ $20.00.
   - Registra Entrada 2: 10 @ $30.00.
   - Registra Salida 1: 5 unidades.
   - Oráculo PEPS: Existencia 15.0000, Valor $400.00, Costo unitario $26.6667 (D-09), 2 capas activas.
   - Oráculo Promedio: Existencia 15.0000, Valor $375.00, Costo unitario $25.0000.
   - Historial: 3 movimientos registrados.
2. `CargarEjercicioE01Async_SegundaVez_NoDuplicaProductoNiMovimientos` (Idempotencia / Criterio 2) -> **PASÓ**
   - Ejecuta una segunda carga y comprueba `YaExistia == true`.
   - Verifica que no se inserte ningún producto, movimiento ni capa adicional.

### Pruebas Funcionales Web (`ConsultaExistenciasPageTests.cs`)
3. `ConsultaModel_OnPostCargarEjercicioE01Async_RedirigeAConsultaConProductoId` -> **PASÓ**
   - Verifica la ejecución de la acción web con un solo clic y redirección automática hacia la vista con el producto cargado.

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
Correctas! - Con error: 0, Superado: 165, Omitido: 0, Total: 165, Duración: 6 s
Suite completa del sistema: 165 / 165 pruebas superadas (100% de éxito).
```

Estado: **VERDE CONFIRMADO**
