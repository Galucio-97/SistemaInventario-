# Evidencia de Fase RED — SI-034: Carga del ejercicio de referencia desde la interfaz

- **Ticket:** SI-034
- **Fecha:** 2026-10-28
- **Rama:** `feat/SI-034-Carga-Ejercicio-Referencia`
- **Requisitos cubiertos:** CP-08, CP-09, RF-04, RF-05

## 1. Pruebas Diseñadas

En [`EjercicioReferenciaIntegrationTests.cs`](file:///C:/Users/gaalz/OneDrive/Documents/GitHub/SistemaInventario/SI.Tests/IntegrationTests/EjercicioReferenciaIntegrationTests.cs):
1. `CargarEjercicioE01Async_PrimeraVez_CreaProductoYMovimientosE01ExactosConOraculo` (CP-08, CP-09)
2. `CargarEjercicioE01Async_SegundaVez_NoDuplicaProductoNiMovimientos` (Idempotencia / Criterio 2)

## 2. Resultado de la Ejecución

```text
Serie de pruebas para SI.Tests.dll (.NETCoreApp,Version=v8.0)
[xUnit.net] SI.Tests.IntegrationTests.EjercicioReferenciaIntegrationTests.CargarEjercicioE01Async_SegundaVez_NoDuplicaProductoNiMovimientos [FAIL]
[xUnit.net] SI.Tests.IntegrationTests.EjercicioReferenciaIntegrationTests.CargarEjercicioE01Async_PrimeraVez_CreaProductoYMovimientosE01ExactosConOraculo [FAIL]

Con error! - Con error: 2, Superado: 0, Omitido: 0, Total: 2, Duración: 127 ms
System.NotImplementedException: Método no implementado aún (Fase RED - SI-034).
```

Estado: **ROJO CONFIRMADO**
