namespace SI.Web.Common;

public static class ValidadorCantidad
{
    public static ResultadoValidacion Validar(decimal cantidad)
    {
        if (cantidad <= 0)
        {
            return new ResultadoValidacion(false, "La cantidad debe ser mayor que cero.");
        }

        if (decimal.Round(cantidad, 4) != cantidad)
        {
            return new ResultadoValidacion(false, "La cantidad no puede tener más de 4 decimales.");
        }

        return new ResultadoValidacion(true);
    }
}
