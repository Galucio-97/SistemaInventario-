namespace SI.Web.Common;

/// <summary>
/// Validador de reglas de dominio para alta y modificación de productos (RF-01, RNF-03).
/// </summary>
public static class ValidadorProducto
{
    public const string MensajeCodigoObligatorio = "El código del producto es obligatorio.";
    public const string MensajeNombreObligatorio = "El nombre del producto es obligatorio.";
    public const string MensajeUnidadObligatoria = "La unidad de medida es obligatoria.";
    public const string MensajeCodigoLargo = "El código no puede tener más de 50 caracteres.";
    public const string MensajeNombreLargo = "El nombre no puede tener más de 150 caracteres.";
    public const string MensajeUnidadLarga = "La unidad de medida no puede tener más de 30 caracteres.";
    public const string MensajeDescripcionLarga = "La descripción no puede tener más de 500 caracteres.";

    public static ResultadoValidacion Validar(string? codigo, string? nombre, string? unidadMedida, string? descripcion = null)
    {
        // Stub para fase RED
        return new ResultadoValidacion(true);
    }
}
