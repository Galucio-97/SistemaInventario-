namespace SI.Web.Common;

public static class ValidadorFechaMovimiento
{
    public const string MensajeFechaRetroactiva =
        "La fecha del movimiento no puede ser anterior al último movimiento registrado.";

    public static ResultadoValidacion Validar(DateTime fechaNuevoMovimiento, DateTime? fechaUltimoMovimiento)
    {
        return fechaUltimoMovimiento.HasValue && fechaNuevoMovimiento < fechaUltimoMovimiento.Value
            ? new ResultadoValidacion(false, MensajeFechaRetroactiva)
            : new ResultadoValidacion(true);
    }
}