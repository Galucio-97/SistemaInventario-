# Evidencia Fase GREEN — SI-028: Invariante de valores almacenados (RNF-01, RF-05, RB-08, CP-19)

## Comando ejecutado (Pruebas de Invariante)
```bash
dotnet test --filter "FullyQualifiedName~InvarianteValoresTests"
```

## Salida de pruebas (GREEN)
```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     3, Omitido:     0, Total:     3, Duración: 147 ms - SI.Tests.dll (net8.0)
```

## Comando ejecutado (Suite Completa)
```bash
dotnet test
```

## Salida Suite Completa
```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  SI.Web -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Web/bin/Debug/net8.0/SI.Web.dll
  SI.Tests -> /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll
Serie de pruebas para /Users/riguidixrodriguez/RiderProjects/SistemaInventario/SI.Tests/bin/Debug/net8.0/SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (arm64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:   132, Omitido:     0, Total:   132, Duración: 2 s - SI.Tests.dll (net8.0)
```

## Verificación de Resultados (`CP-19`)

### 1. Escenario E-01 (Transiciones verificadas paso a paso)
- **Tras E1:**
  - `ExistenciaActual`: 10.0000 = `10 - 0` = `ExistenciaResultante` de E1.
  - `ValorInventarioPeps`: $200.00 = Capa 1 (10 × 20.00) = `SaldoValorPeps` de E1.
  - `ValorInventarioPromedio`: $200.00 = `SaldoValorPromedio` de E1.
- **Tras E2:**
  - `ExistenciaActual`: 20.0000 = `(10 + 10) - 0` = `ExistenciaResultante` de E2.
  - `ValorInventarioPeps`: $500.00 = Capa 1 (10 × 20.00) + Capa 2 (10 × 30.00) = `SaldoValorPeps` de E2.
  - `ValorInventarioPromedio`: $500.00 = `SaldoValorPromedio` de E2.
- **Tras S1:**
  - `ExistenciaActual`: 15.0000 = `(10 + 10) - 5` = `ExistenciaResultante` de S1.
  - `ValorInventarioPeps`: $400.00 = Capa 1 (5 × 20.00) + Capa 2 (10 × 30.00) = `SaldoValorPeps` de S1.
  - `ValorInventarioPromedio`: $375.00 = `SaldoValorPromedio` de S1.

### 2. Escenario E-02 (Agotamiento de capas y ajuste D-03)
- **Tras S1 (multicapa):**
  - Existencia: 2.0000 = Capa 2 (2 × 11.00) = $22.00 = Saldo PEPS de S1.
  - Saldo Promedio: $21.14 = Saldo Promedio de S1.
- **Tras S2 (existencia 0):**
  - Existencia: 0.0000 = `(3 + 4) - (5 + 2)` = `ExistenciaResultante` de S2.
  - Saldo PEPS: $0.00 = 0 capas disponibles = Saldo PEPS de S2.
  - Saldo Promedio: $0.00 = Saldo Promedio de S2.

### 3. Falla ante violaciones de invariantes
- Si se adultera el valor PEPS, el valor Promedio o la Existencia física de la entidad `Producto`, la validación falla inmediatamente, demostrando la sensibilidad del test ante corrupciones de datos.
