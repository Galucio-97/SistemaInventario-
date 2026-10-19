# Evidencia Técnica — SI-015: Promedio Ponderado: Entradas

## 1. Resumen del Ticket
- **ID:** `SI-015`
- **Release:** `v0.2.0` (Reglas y motor de costeo)
- **Responsable:** Dev B (Gustavo Lucio)
- **Objetivo:** Recalcular el costo promedio móvil y el saldo patrimonial en cada entrada mediante la fórmula del enunciado (**RF-04**, **D-10**).
- **Trazabilidad:** `RF-04` (Métodos de costeo dual), `D-10` (Fórmula canónica de promedio ponderado), `D-01` (Escala decimal 4 decimales), `D-02` (Redondeo bancario ToEven), `CP-09`.

---

## 2. Escenarios y Oráculos Verificados

### 2.1 Escenario E-01 (Referencia Canónica Sección 6.3)
- **E1:** 10 unidades @ 20.00 sobre existencia inicial 0
  - Existencia Resultante: `10`
  - Saldo Resultante: `200.00`
  - Costo Promedio Resultante: `20.0000`
- **E2:** 10 unidades @ 30.00
  - Valor Entrada: `300.00`
  - Existencia Resultante: `20`
  - Saldo Resultante: `200.00 + 300.00 = 500.00`
  - Costo Promedio: `500.00 / 20 = 25.0000`

### 2.2 Escenario E-02 (Promedio Periódico con Redondeo a 4 Decimales)
- **E2:** Con 3 unidades previas y saldo 30.00, entra 4 @ 11.00
  - Valor Entrada: `44.00`
  - Existencia Resultante: `7`
  - Saldo Resultante: `74.00`
  - Costo Promedio: `74.00 / 7 = 10.571428...` $\rightarrow$ **`10.5714`**

### 2.3 Escenario E-03 (Redondeo Bancario Exacto ToEven)
- **E2:** Con 3 unidades previas y saldo 30.00, entra 3 @ 10.01
  - Valor Entrada: `30.03`
  - Existencia Resultante: `6`
  - Saldo Resultante: `60.03`
  - Costo Promedio: `60.03 / 6 = 10.0050` exacto $\rightarrow$ **`10.0050`** (redondeo bancario conserva el dígito par)

### 2.4 Caso Borde (Existencia Cero)
- Si la existencia resultante es 0, el método retorna costo promedio `0.0000` de forma segura sin disparar `DivideByZeroException`.

---

## 3. Implementación Refactorizada (`SI.Web/Services/CalculadoraPromedioPonderado.cs`)

```csharp
namespace SI.Web.Services;

/// <summary>
/// Resultado del cálculo de inventario promedio ponderado tras una entrada.
/// </summary>
public record ResultadoEntradaPromedio(
    decimal Existencia,
    decimal SaldoValor,
    decimal CostoPromedio
);

/// <summary>
/// Motor de cálculo para valuación de inventarios por Costo Promedio Ponderado Móvil (RF-04, D-10).
/// </summary>
public static class CalculadoraPromedioPonderado
{
    public static ResultadoEntradaPromedio CalcularEntrada(
        decimal existenciaActual,
        decimal saldoValorActual,
        decimal cantidadEntrada,
        decimal costoUnitarioEntrada)
    {
        var valorEntrada = PoliticaRedondeo.Importe(cantidadEntrada * costoUnitarioEntrada);
        var nuevaExistencia = existenciaActual + cantidadEntrada;
        var nuevoSaldoValor = PoliticaRedondeo.Importe(saldoValorActual + valorEntrada);

        var nuevoCostoPromedio = nuevaExistencia > 0
            ? PoliticaRedondeo.CostoUnitario(nuevoSaldoValor / nuevaExistencia)
            : 0m;

        return new ResultadoEntradaPromedio(nuevaExistencia, nuevoSaldoValor, nuevoCostoPromedio);
    }
}
```

---

## 4. Ejecución y Resultados

```bash
dotnet test --filter "CP=CP-09" --no-restore
```
- Total de pruebas: 5
- Superadas: 5 (100%)
- Fallidas: 0
- Duración: ~5 ms
