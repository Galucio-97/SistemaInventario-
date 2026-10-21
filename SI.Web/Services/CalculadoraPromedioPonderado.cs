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
/// Resultado del cálculo de inventario promedio ponderado tras una salida.
/// </summary>
/// <param name="Existencia">Existencia física resultante a 4 decimales.</param>
/// <param name="SaldoValor">Saldo patrimonial valorizado resultante a 2 decimales.</param>
/// <param name="CostoPromedio">Costo promedio unitario resultante a 4 decimales.</param>
/// <param name="CostoTotalSalida">Costo total atribuido a la salida a 2 decimales.</param>
public record ResultadoSalidaPromedio(
    decimal Existencia,
    decimal SaldoValor,
    decimal CostoPromedio,
    decimal CostoTotalSalida
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

    /// <summary>
    /// Calcula el costo de salida y actualiza saldo y existencias según Costo Promedio Móvil y regla D-03.
    /// </summary>
    public static ResultadoSalidaPromedio CalcularSalida(
        decimal existenciaActual,
        decimal saldoValorActual,
        decimal costoPromedioActual,
        decimal cantidadSalida)
    
    {
        if (cantidadSalida > existenciaActual)
        {
            throw new InvalidOperationException($"La cantidad de salida ({cantidadSalida}) " +
              $"no puede exceder la existencia disponible ({existenciaActual}).");
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
}
