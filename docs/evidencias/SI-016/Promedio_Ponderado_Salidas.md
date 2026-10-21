# Evidencia Técnica — SI-016: Promedio Ponderado: Salidas y Ajuste por Residuo

## 1. Resumen del Ticket
- **ID:** `SI-016`
- **Release:** `v0.2.0` (Reglas y motor de costeo)
- **Responsable:** Dev B (Gustavo Lucio)
- **Objetivo:** Costear salidas con el promedio ponderado móvil vigente sin alterarlo, y garantizar que existencia 0 implique saldo de valor 0.00 mediante la aplicación estricta de la regla **D-03** (**RF-04**, **RB-07**, **RB-09**, **RNF-01**, **D-03**).
- **Trazabilidad:** `RF-04` (Métodos de costeo dual), `RB-07` (Costo según método), `RB-09` (Redondeo documentado), `RNF-01` (Integridad de datos), `D-03` (Ajuste por residuo cuando existencia llega a cero), `CP-09`, `CP-17`.

---

## 2. Escenarios y Oráculos Verificados

### 2.1 Escenario E-01 (Salida Parcial)
- **S1 (5 unidades sobre 20 unidades previas, saldo 500.00, promedio 25.0000):**
  - Costo de Salida: $5 \times 25.0000 = \mathbf{125.00}$
  - Saldo Remanente: $500.00 - 125.00 = \mathbf{375.00}$
  - Existencia Remanente: $20 - 5 = \mathbf{15}$
  - Promedio Vigente: se mantiene en $\mathbf{25.0000}$ (la salida no altera el costo unitario de las unidades restantes).

### 2.2 Escenario E-02 (Salida con Promedio Periódico y Agotamiento de Stock)
- **S1 (5 unidades sobre 7 previas, saldo 74.00, promedio 10.5714):**
  - Costo de Salida: $\text{Importe}(5 \times 10.5714) = 52.857 \rightarrow \mathbf{52.86}$
  - Saldo Remanente: $74.00 - 52.86 = \mathbf{21.14}$
  - Existencia Remanente: $\mathbf{2}$
- **S2 (2 unidades, agota existencia a 0):**
  - Existencia Resultante: $\mathbf{0}$
  - Costo Salida: $\mathbf{21.14}$ (absorbe el saldo restante)
  - Saldo Resultante: $\mathbf{0.00}$
  - Promedio Resultante: $\mathbf{0.0000}$

### 2.3 Escenario E-03 (Redondeo Bancario y Demostración de Ajuste por Residuo D-03)
- **S1 (1 unidad sobre 6 previas, saldo 60.03, promedio 10.0050):**
  - Costo Salida: $1 \times 10.0050 = 10.005 \rightarrow$ redondeo bancario to-even a par $\rightarrow \mathbf{10.00}$ (el redondeo half-up tradicional habría dado 10.01).
  - Saldo Remanente: $60.03 - 10.00 = \mathbf{50.03}$
  - Existencia: $\mathbf{5}$
- **S2 (5 unidades, agota existencia a 0):**
  - Sin D-03: $5 \times 10.0050 = 50.025 \rightarrow$ bancario $50.02$, dejando un saldo remanente corrupto de $0.01$ con 0 unidades.
  - **Con Regla D-03:** Al llegar la existencia a 0, la salida absorbe el saldo restante completo $\mathbf{50.03}$, garantizando que el inventario quede en $\mathbf{0.00}$.

### 2.4 Control Defensivo de Dominio
- Si `cantidadSalida > existenciaActual`, el cálculo falla de inmediato con `InvalidOperationException`, previniendo saldos o existencias negativas en la capa de dominio.

---

## 3. Implementación Refactorizada (`SI.Web/Services/CalculadoraPromedioPonderado.cs`)

```csharp
public static ResultadoSalidaPromedio CalcularSalida(
    decimal existenciaActual,
    decimal saldoValorActual,
    decimal costoPromedioActual,
    decimal cantidadSalida)
{
    if (cantidadSalida > existenciaActual)
    {
        throw new InvalidOperationException($"La cantidad de salida ({cantidadSalida}) no puede exceder la existencia disponible ({existenciaActual}).");
    }

    var nuevaExistencia = existenciaActual - cantidadSalida;

    if (nuevaExistencia == 0)
    {
        // Regla D-03: Ajuste por residuo. La salida absorbe el saldo restante completo y el saldo queda estrictamente en 0.00.
        return new ResultadoSalidaPromedio(0m, 0.00m, 0.0000m, saldoValorActual);
    }

    var costoTotalSalida = PoliticaRedondeo.Importe(cantidadSalida * costoPromedioActual);
    var nuevoSaldoValor = PoliticaRedondeo.Importe(saldoValorActual - costoTotalSalida);

    return new ResultadoSalidaPromedio(nuevaExistencia, nuevoSaldoValor, costoPromedioActual, costoTotalSalida);
}
```

---

## 4. Ejecución y Resultados

```bash
dotnet test --filter "FullyQualifiedName~CalcularSalida" --no-restore
```
- Total de pruebas: 6
- Superadas: 6 (100%)
- Fallidas: 0
- Duración: ~6 ms
