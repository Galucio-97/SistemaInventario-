using SI.Web.Data.Entities;

namespace SI.Web.Services;

/// <summary>
/// Resultado del cálculo de valuación de inventario PEPS tras una salida de almacén.
/// </summary>
/// <param name="CostoTotalSalida">Costo total atribuido a la salida valuado con las capas más antiguas.</param>
/// <param name="SaldoValorRestante">Saldo patrimonial restante del inventario tras la salida.</param>
/// <param name="ExistenciaRestante">Existencia física total disponible restante.</param>
/// <param name="CapasResultantes">Lista de capas con sus cantidades disponibles actualizadas.</param>
/// <param name="CostoUnitarioEquivalente">Costo unitario equivalente ponderado de la salida (CostoTotal / Cantidad) a 4 decimales.</param>
public record ResultadoSalidaPeps(
    decimal CostoTotalSalida,
    decimal SaldoValorRestante,
    decimal ExistenciaRestante,
    IReadOnlyList<CapasPeps> CapasResultantes,
    decimal CostoUnitarioEquivalente = 0m
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
    /// Soporta salidas multicapa (CP-15), agotamiento de inventario (CP-16) y cálculo de costo unitario equivalente.
    /// </summary>
    public static ResultadoSalidaPeps CalcularSalida(IList<CapasPeps> capas, decimal cantidadSalida)
    {
        if (cantidadSalida <= 0 || !capas.Any(c => c.CantidadDisponible > 0))
        {
            var existenciaActual = capas.Sum(c => c.CantidadDisponible);
            var saldoActual = existenciaActual == 0 ? 0m : CalcularValorInventario(capas);
            return new ResultadoSalidaPeps(0m, saldoActual, existenciaActual, capas.ToList(), 0m);
        }

        var capasDisponibles = capas
            .Where(c => c.CantidadDisponible > 0)
            .OrderBy(c => c.FechaEntrada)
            .ThenBy(c => c.Id);

        decimal remanente = cantidadSalida;
        decimal costoTotal = 0m;

        foreach (var capa in capasDisponibles)
        {
            if (remanente <= 0)
                break;

            var aTomar = Math.Min(capa.CantidadDisponible, remanente);
            capa.CantidadDisponible -= aTomar;
            costoTotal += PoliticaRedondeo.Importe(aTomar * capa.CostoUnitario);
            remanente -= aTomar;
        }

        var existenciaRestante = capas.Sum(c => c.CantidadDisponible);
        var saldoValorRestante = existenciaRestante == 0 ? 0m : CalcularValorInventario(capas);
        var costoUnitarioEquivalente = cantidadSalida > 0
            ? PoliticaRedondeo.CostoUnitario(costoTotal / cantidadSalida)
            : 0m;

        return new ResultadoSalidaPeps(
            costoTotal,
            saldoValorRestante,
            existenciaRestante,
            capas.ToList(),
            costoUnitarioEquivalente
        );
    }
}
