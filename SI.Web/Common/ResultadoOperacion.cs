namespace SI.Web.Common;

/// <summary>
/// Representa el resultado de una operación de servicio en el dominio.
/// </summary>
/// <param name="Exitoso">Indica si la operación concluyó exitosamente.</param>
/// <param name="MensajeError">Mensaje descriptivo en caso de error o rechazo.</param>
/// <param name="EntidadId">Identificador asignado a la entidad creada (si aplica).</param>
public record ResultadoOperacion(
    bool Exitoso,
    string? MensajeError = null,
    int? EntidadId = null
);
