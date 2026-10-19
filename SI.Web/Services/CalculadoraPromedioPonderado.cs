namespace SI.Web.Services;

/// <summary>
/// Resultado del cálculo de inventario promedio ponderado tras una entrada.
/// </summary>
/// <param name="Existencia">Existencia física resultante a 4 decimales (decimal(18,4)).</param>
/// <param name="SaldoValor">Saldo patrimonial valorizado resultante a 2 decimales (decimal(18,2)).</param>
/// <param name="CostoPromedio">Costo promedio móvil resultante a 4 decimales (decimal(18,4)).</param>
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
    /// <summary>
    /// Recalcula el costo promedio móvil y el saldo patrimonial tras un movimiento de entrada.
    /// Aplica la fórmula canónica: Nuevo Promedio = (Saldo Anterior + Valor Entrada) / (Existencia Anterior + Cantidad Entrada).
    /// </summary>
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
