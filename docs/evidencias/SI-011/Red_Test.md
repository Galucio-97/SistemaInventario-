# Evidencia de Prueba RED — SI-011

## 1. Resumen

- **Ticket:** `SI-011` — *Política de redondeo bancario* (release `v0.2.0`).
- **Caso de prueba:** `CP-17` (parte de redondeo) · **Reglas:** `RB-08`, `RB-09` · **Decisión:** `D-02` (redondeo bancario, `ToEven`).
- **Fase TDD:** RED — las pruebas se escribieron **antes** de que la implementación fuera correcta y fallaron.
- **Fecha:** 2026-10-18 · **Ejecutor:** Dev B · **Rama:** `feat/SI-011-Politica-de-redondeo-bancario`.
- **Archivo de pruebas:** `SI.Tests/UnitTests/PoliticaRedondeoTests.cs`.

## 2. Ambiente

| Parámetro | Valor |
|---|---|
| Sistema operativo | Windows (terminal Git Bash) |
| .NET SDK | `8.0.426` |
| Herramienta de pruebas | xUnit 2.5.3 · VSTest `17.11.1 (x64)` |

## 3. Comando ejecutado

```bash
dotnet test --filter="CP=CP-17"
```

## 4. Oráculo (resultados esperados)

Calculados a mano según la tabla 5.1 de `docs/02-requisitos-y-decisiones.md` y el ticket SI-011 del backlog.

| Método | Entrada | Esperado | Razonamiento (`ToEven`) |
|---|---|---|---|
| `Importe` (2 dec.) | `10.005` | `10.00` | El dígito anterior (0) es par → se conserva |
| `Importe` (2 dec.) | `10.015` | `10.02` | El dígito anterior (1) es impar → sube a par |
| `Importe` (2 dec.) | `10.0051` | `10.01` | Hay dígitos distintos de cero después del 5 → sube |
| `Importe` (2 dec.) | `50.025` | `50.02` | El dígito anterior (2) es par → se conserva |
| `CostoUnitario` (4 dec.) | `10.00005` | `10.0000` | El dígito anterior (0) es par → se conserva |
| `CostoUnitario` (4 dec.) | `10.00015` | `10.0002` | El dígito anterior (1) es impar → sube a par |

## 5. Resultado obtenido en RED

**Las 6 pruebas fallaron.** En todas, el valor obtenido no coincide con el esperado:

| Prueba | Entrada | Esperado | Obtenido | Estado |
|---|---|---|---|---|
| `Importe_RedondeoBancario_A2Decimales` | `10.005` | `10.00` | `0.10005` | **FAIL** |
| `Importe_RedondeoBancario_A2Decimales` | `10.015` | `10.02` | `0.10015` | **FAIL** |
| `Importe_RedondeoBancario_A2Decimales` | `10.0051` | `10.01` | `0.100051` | **FAIL** |
| `Importe_RedondeoBancario_A2Decimales` | `50.025` | `50.02` | `0.50025` | **FAIL** |
| `CostoUnitario_RedondeoBancario_A4Decimales` | `10.00005` | `10.0000` | `0.1000005` | **FAIL** |
| `CostoUnitario_RedondeoBancario_A4Decimales` | `10.00015` | `10.0002` | `0.1000015` | **FAIL** |

### Salida de consola (extracto)

```text
Serie de pruebas para ...\SI.Tests\bin\Debug\net8.0\SI.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.
[xUnit.net 00:00:01.28]   SI.Tests.UnitTests.PoliticaRedondeoTests.Importe_RedondeoBancario_A2Decimales(entrada: 10.0051, esperado: 10.01) [FAIL]
[xUnit.net 00:00:01.33]   SI.Tests.UnitTests.PoliticaRedondeoTests.Importe_RedondeoBancario_A2Decimales(entrada: 10.015, esperado: 10.02) [FAIL]
[xUnit.net 00:00:01.33]   SI.Tests.UnitTests.PoliticaRedondeoTests.Importe_RedondeoBancario_A2Decimales(entrada: 10.005, esperado: 10.00) [FAIL]
[xUnit.net 00:00:01.33]   SI.Tests.UnitTests.PoliticaRedondeoTests.Importe_RedondeoBancario_A2Decimales(entrada: 50.025, esperado: 50.02) [FAIL]
[xUnit.net 00:00:01.33]   SI.Tests.UnitTests.PoliticaRedondeoTests.CostoUnitario_RedondeoBancario_A4Decimales(entrada: 10.00015, esperado: 10.0002) [FAIL]
[xUnit.net 00:00:01.33]   SI.Tests.UnitTests.PoliticaRedondeoTests.CostoUnitario_RedondeoBancario_A4Decimales(entrada: 10.00005, esperado: 10.0000) [FAIL]

  Con error SI.Tests.UnitTests.PoliticaRedondeoTests.Importe_RedondeoBancario_A2Decimales(entrada: 10.0051, esperado: 10.01)
  Mensaje de error:
   Assert.Equal() Failure: Values differ
Expected: 10.01
Actual:   0.100051

  (... mismo patrón "Values differ" en las otras 5 pruebas, ver tabla ...)

Con error! - Con error:     6, Superado:     0, Omitido:     0, Total:     6, Duración: 52 ms - SI.Tests.dll (net8.0)
```

## 6. Conclusión de la fase RED

- [x] Las pruebas compilan y se ejecutan.
- [x] Las 6 fallan por **resultado incorrecto** (no por un error de infraestructura), por lo que verifican realmente el comportamiento esperado.
- [x] Esto **no es un defecto** del producto: es el ciclo normal de TDD (`docs/01-plan-de-pruebas.md` §3, "RED porque la funcionalidad aún no existe/no es correcta").
- Siguiente paso: implementar `PoliticaRedondeo` con `MidpointRounding.ToEven` (ver `Green_Test.md`).
