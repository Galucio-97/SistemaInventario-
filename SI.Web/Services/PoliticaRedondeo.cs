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