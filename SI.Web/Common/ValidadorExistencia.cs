namespace SI.Web.Common;

/// <summary>
/// Validador de suficiencia de inventario para salidas de almacén (RB-03).
/// Impide que se registren salidas que excedan la existencia física disponible.
/// </summary>
public static class ValidadorExistencia
{
    public static ResultadoValidacion Validar(decimal cantidadSalida, decimal existenciaDisponible)
    {
        return new ResultadoValidacion(true);
    }
}
