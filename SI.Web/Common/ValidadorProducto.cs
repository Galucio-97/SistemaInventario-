namespace SI.Web.Common;

/// <summary>
/// Validador de reglas de dominio para alta y modificación de productos (RF-01, RNF-03).
/// </summary>
public static class ValidadorProducto
{
    public const int MaxLongitudCodigo = 50;
    public const int MaxLongitudNombre = 150;
    public const int MaxLongitudUnidad = 30;
    public const int MaxLongitudDescripcion = 500;

    public const string MensajeCodigoObligatorio = "El código del producto es obligatorio.";
    public const string MensajeNombreObligatorio = "El nombre del producto es obligatorio.";
    public const string MensajeUnidadObligatoria = "La unidad de medida es obligatoria.";
    public const string MensajeCodigoLargo = "El código no puede tener más de 50 caracteres.";
    public const string MensajeNombreLargo = "El nombre no puede tener más de 150 caracteres.";
    public const string MensajeUnidadLarga = "La unidad de medida no puede tener más de 30 caracteres.";
    public const string MensajeDescripcionLarga = "La descripción no puede tener más de 500 caracteres.";

    private static readonly ResultadoValidacion ResultadoValido = new(true);

    public static ResultadoValidacion Validar(string? codigo, string? nombre, string? unidadMedida, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return new ResultadoValidacion(false, MensajeCodigoObligatorio);
        }

        if (codigo.Length > MaxLongitudCodigo)
        {
            return new ResultadoValidacion(false, MensajeCodigoLargo);
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            return new ResultadoValidacion(false, MensajeNombreObligatorio);
        }

        if (nombre.Length > MaxLongitudNombre)
        {
            return new ResultadoValidacion(false, MensajeNombreLargo);
        }

        if (string.IsNullOrWhiteSpace(unidadMedida))
        {
            return new ResultadoValidacion(false, MensajeUnidadObligatoria);
        }

        if (unidadMedida.Length > MaxLongitudUnidad)
        {
            return new ResultadoValidacion(false, MensajeUnidadLarga);
        }

        if (!string.IsNullOrEmpty(descripcion) && descripcion.Length > MaxLongitudDescripcion)
        {
            return new ResultadoValidacion(false, MensajeDescripcionLarga);
        }

        return ResultadoValido;
    }

    public static ResultadoValidacion ValidarModificacion(string? nombre, string? unidadMedida, string? descripcion = null)
    {
        // Stub inicial para fase RED
        return new ResultadoValidacion(false, "No implementado");
    }
}
