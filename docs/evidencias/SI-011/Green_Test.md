# Evidencia de Prueba GREEN — SI-011

## 1. Resumen

- **Ticket:** `SI-011` — *Política de redondeo bancario* (release `v0.2.0`).
- **Caso de prueba:** `CP-17` (parte de redondeo) · **Reglas:** `RB-08`, `RB-09` · **Decisión:** `D-02`.
- **Fase TDD:** GREEN — con la implementación de `PoliticaRedondeo`, las 6 pruebas del RED pasan.
- **Fecha:** 2026-10-18 · **Ejecutor:** Dev B · **Rama:** `feat/SI-011-Politica-de-redondeo-bancario`.

## 2. Implementación

Archivo: `SI.Web/Services/PoliticaRedondeo.cs`

```csharp
namespace SI.Web.Services;

public static class PoliticaRedondeo
{
    /// <summary>
    /// Redondea importes monetarios y saldos a 2 decimales usando redondeo bancario.
    /// </summary>
    public static decimal Importe(decimal valor) =>
        Math.Round(valor, 2, MidpointRounding.ToEven);


    /// <summary>
    /// Redondea costos unitarios a 4 decimales usando redondeo bancario.
    /// </summary>
    public static decimal CostoUnitario(decimal valor) =>
        Math.Round(valor, 4, MidpointRounding.ToEven);
}
```

**Decisiones de diseño** (`docs/02-requisitos-y-decisiones.md` §5.3):
- `MidpointRounding.ToEven` se especifica **de forma explícita**, sin depender del valor por defecto.
- Escalas según §5.2: costo unitario → 4 decimales, importes y saldos → 2 decimales.
- Es el **único** punto de redondeo de `SI.Web`: una búsqueda de `Math.Round` en el proyecto solo encuentra estas dos líneas (criterio de aceptación 4).

## 3. Comando ejecutado

```bash
dotnet test --filter "CP=CP-17" --verbosity minimal
```

## 4. Resultado obtenido en GREEN

**Las 6 pruebas pasan.**

| Prueba | Entrada | Esperado | Obtenido | Estado |
|---|---|---|---|---|
| `Importe_RedondeoBancario_A2Decimales` | `10.005` | `10.00` | `10.00` | **PASS** |
| `Importe_RedondeoBancario_A2Decimales` | `10.015` | `10.02` | `10.02` | **PASS** |
| `Importe_RedondeoBancario_A2Decimales` | `10.0051` | `10.01` | `10.01` | **PASS** |
| `Importe_RedondeoBancario_A2Decimales` | `50.025` | `50.02` | `50.02` | **PASS** |
| `CostoUnitario_RedondeoBancario_A4Decimales` | `10.00005` | `10.0000` | `10.0000` | **PASS** |
| `CostoUnitario_RedondeoBancario_A4Decimales` | `10.00015` | `10.0002` | `10.0002` | **PASS** |

### Salida de consola

```text
Determinando los proyectos que se van a restaurar...
Todos los proyectos están actualizados para la restauración.
SI.Web -> ...\SI.Web\bin\Debug\net8.0\SI.Web.dll
SI.Tests -> ...\SI.Tests\bin\Debug\net8.0\SI.Tests.dll
Serie de pruebas para ...\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:     6, Omitido:     0, Total:     6, Duración: 4 ms - SI.Tests.dll (net8.0)
```

## 5. Comparación RED → GREEN

| | RED | GREEN |
|---|---|---|
| Superadas | 0 | **6** |
| Con error | 6 | **0** |
| Total | 6 | 6 |

## 6. Criterios de aceptación de SI-011

- [x] Hay redondeo de costo unitario a 4 decimales y de importe a 2, ambos bancarios (`ToEven` explícito).
- [x] Pruebas parametrizadas con la tabla 5.1: `10.005 → 10.00`, `10.015 → 10.02`, `10.0051 → 10.01`, `50.025 → 50.02`.
- [x] Pruebas a 4 decimales: `10.00005 → 10.0000`, `10.00015 → 10.0002`.
- [x] Ningún otro componente redondea por su cuenta *(verificado por búsqueda de `Math.Round`; pendiente de confirmar en la revisión del PR)*.

> **Alcance sobre CP-17:** estas pruebas cubren únicamente la política de redondeo. El escenario E-03 completo (salida con ajuste por residuo, D-03) depende de las calculadoras de SI-015/SI-016, por lo que CP-17 **no** se marca como Aprobado con este ticket.
