namespace SI.Web.Common;

/// <summary>
/// Valida las reglas de dominio para costo unitario en movimientos de entrada (RB-02, D-01).
/// </summary>
public static class ValidadorCostoUnitario
{
    private const string MensajeCostoMenorOIgualACero = "El costo unitario debe ser mayor que cero.";
    private const string MensajePrecisionExcedida = "El costo unitario no puede tener más de 4 decimales.";

    private static readonly ResultadoValidacion ResultadoValido = new(true);

    public static ResultadoValidacion Validar(decimal costoUnitario)
    {
        if (costoUnitario <= 0)
        {
            return new ResultadoValidacion(false, MensajeCostoMenorOIgualACero);
        }

        return decimal.Round(costoUnitario, 4) != costoUnitario 
            ? new ResultadoValidacion(false, MensajePrecisionExcedida) 
            : ResultadoValido;
    }
}