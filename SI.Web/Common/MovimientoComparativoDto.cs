using SI.Web.Enums;

namespace SI.Web.Common;

/// <summary>
/// Representa una fila del historial comparativo en paralelo PEPS vs Promedio Ponderado (RF-04, CP-10, SI-035).
/// </summary>
public sealed record MovimientoComparativoDto
{
    public int Id { get; init; }
    public DateTime Fecha { get; init; }
    public TipoMovimiento Tipo { get; init; }
    
    // Cantidades físicas comunes (estrictamente idénticas)
    public decimal Cantidad { get; init; }
    public decimal ExistenciaResultante { get; init; }

    // Columnas Método PEPS
    public decimal CostoUnitarioPeps { get; init; }
    public decimal CostoTotalPeps { get; init; }
    public decimal SaldoValorPeps { get; init; }

    // Columnas Método Promedio Ponderado
    public decimal CostoUnitarioPromedio { get; init; }
    public decimal CostoTotalPromedio { get; init; }
    public decimal SaldoValorPromedio { get; init; }

    // Diferencia patrimonial (PEPS - Promedio)
    public decimal DiferenciaSaldoValor => SaldoValorPeps - SaldoValorPromedio;
    public decimal DiferenciaCostoTotal => CostoTotalPeps - CostoTotalPromedio;
}
