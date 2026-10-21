using SI.Web.Data.Entities;

namespace SI.Web.Services;

/// <summary>
/// Resultado del cálculo de valuación de inventario PEPS tras una salida de almacén.
/// </summary>
/// <param name="CostoTotalSalida">Costo total atribuido a la salida valuado con las capas más antiguas.</param>
/// <param name="SaldoValorRestante">Saldo patrimonial restante del inventario tras la salida.</param>
/// <param name="ExistenciaRestante">Existencia física total disponible restante.</param>
/// <param name="CapasResultantes">Lista de capas con sus cantidades disponibles actualizadas.</param>
public record ResultadoSalidaPeps(
    decimal CostoTotalSalida,
    decimal SaldoValorRestante,
    decimal ExistenciaRestante,
    IReadOnlyList<CapasPeps> CapasResultantes
);

/// <summary>
/// Motor de cálculo para valuación de inventarios por PEPS / FIFO (Primeras Entradas, Primeras Salidas).
/// Asigna a las salidas el costo de adquisición de las unidades que ingresaron primero (RF-04, RB-07).
/// </summary>
public static class CalculadoraPeps
{
    /// <summary>
    /// Crea una nueva capa de inventario a partir de una entrada de mercancía.
    /// </summary>
    public static CapasPeps CrearCapa(
        DateTime fecha,
        decimal cantidad,
        decimal costoUnitario,
        int productoId = 0,
        int movimientoId = 0)
    {
        return new CapasPeps
        {
            FechaEntrada = fecha,
            CantidadInicial = cantidad,
            CantidadDisponible = cantidad,
            CostoUnitario =  costoUnitario,
            ProductoId = productoId,
            MovimientoId = movimientoId
        };
    }

    /// <summary>
    /// Calcula el valor total patrimonial del inventario sumando (CantidadDisponible * CostoUnitario) de cada capa.
    /// </summary>
    public static decimal CalcularValorInventario(IEnumerable<CapasPeps> capas)
    {
        return capas.Sum(capa => 
            PoliticaRedondeo.Importe(capa.CantidadDisponible * capa.CostoUnitario)
        );
    }

    /// <summary>
    /// Consume existencias de las capas en estricto orden cronológico (FechaEntrada ASC, Id ASC).
    /// En SI-017: cuando la salida cabe en la capa más antigua, consume únicamente de ella.
    /// </summary>
    public static ResultadoSalidaPeps CalcularSalida(IList<CapasPeps> capas, decimal cantidadSalida)
    {
        var capaMasAntigua = capas
            .Where(c => c.CantidadDisponible > 0)
            .OrderBy(c => c.FechaEntrada)
            .ThenBy(c => c.Id)
            .FirstOrDefault();

        if (capaMasAntigua is null || cantidadSalida <= 0)
        {
            return new ResultadoSalidaPeps(0m, CalcularValorInventario(capas), capas.Sum(c => c.CantidadDisponible), capas.ToList());
        }

        capaMasAntigua.CantidadDisponible -= cantidadSalida;
        var costoTotalSalida = PoliticaRedondeo.Importe(cantidadSalida * capaMasAntigua.CostoUnitario);
        var saldoValorRestante = CalcularValorInventario(capas);
        var existenciaRestante = capas.Sum(c => c.CantidadDisponible);

        return new ResultadoSalidaPeps(costoTotalSalida, saldoValorRestante, existenciaRestante, capas.ToList());
    }
}
