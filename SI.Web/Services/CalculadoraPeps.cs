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
        // TODO (GREEN): Implementar creación de capa con CantidadInicial y CantidadDisponible
        return new CapasPeps();
    }

    /// <summary>
    /// Calcula el valor total patrimonial del inventario sumando (CantidadDisponible * CostoUnitario) de cada capa.
    /// </summary>
    public static decimal CalcularValorInventario(IEnumerable<CapasPeps> capas)
    {
        // TODO (GREEN): Implementar suma de capas con PoliticaRedondeo.Importe
        return 0m;
    }

    /// <summary>
    /// Consume existencias de las capas en estricto orden cronológico (FechaEntrada ASC, Id ASC).
    /// En SI-017: cuando la salida cabe en la capa más antigua, consume únicamente de ella.
    /// </summary>
    public static ResultadoSalidaPeps CalcularSalida(IList<CapasPeps> capas, decimal cantidadSalida)
    {
        // TODO (GREEN): Implementar consumo FIFO de capas y cálculo de costo y saldo
        return new ResultadoSalidaPeps(0m, 0m, 0m, Array.Empty<CapasPeps>());
    }
}
