using SI.Web.Data.Entities;

namespace SI.Web.Common;

/// <summary>
/// Representa el resultado de una consulta individual de producto (RF-01, RF-05, CP-11).
/// </summary>
/// <param name="Encontrado">Indica si el producto fue localizado en el sistema.</param>
/// <param name="Producto">Entidad del producto recuperada (si existe).</param>
/// <param name="MensajeError">Mensaje informativo en caso de que no exista.</param>
public record ResultadoConsultaProducto(
    bool Encontrado,
    Producto? Producto = null,
    string? MensajeError = null
);
