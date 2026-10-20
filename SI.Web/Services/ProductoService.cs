using Microsoft.EntityFrameworkCore;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Web.Services;

/// <summary>
/// Servicio de dominio para administración de productos (RF-01, SI-020).
/// </summary>
public class ProductoService(InventarioDbContext context)
{
    public async Task<ResultadoOperacion> RegistrarAsync(
        string codigo,
        string nombre,
        string unidadMedida,
        string? descripcion = null,
        CancellationToken cancellationToken = default)
    {
        // Stub inicial para fase RED
        return await Task.FromResult(new ResultadoOperacion(false, "No implementado"));
    }
}
