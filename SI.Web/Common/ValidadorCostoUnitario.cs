namespace SI.Web.Common;

public static class ValidadorCostoUnitario
{
    public static ResultadoValidacion Validar(decimal costoUnitario)
    {
        return new ResultadoValidacion(true);
    }
}