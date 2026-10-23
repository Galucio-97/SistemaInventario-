using Microsoft.EntityFrameworkCore;
using SI.Web.Common;
using SI.Web.Data;
using SI.Web.Data.Entities;
using SI.Web.Enums;

namespace SI.Web.Services;

/// <summary>
/// Servicio de dominio para registro transaccional de movimientos de inventario (RF-02, RF-03, RB-04..07, SI-024, SI-025).
/// </summary>
public class InventarioService(InventarioDbContext context)
{
    public const string MensajeProductoNoExiste = "El producto no existe";
    public const string MensajeProductoInactivo = "El producto está inactivo";

    /// <summary>
    /// Registra una entrada de inventario de forma atómica actualizando existencias, capas PEPS y promedio ponderado (RF-02, SI-024).
    /// </summary>
    public async Task<ResultadoOperacion> RegistrarEntradaAsync(
        int productoId,
        DateTime fecha,
        decimal cantidad,
        decimal costoUnitario,
        CancellationToken cancellationToken = default)
    {
        // Stub inicial para fase RED
        return await Task.FromResult(new ResultadoOperacion(false, "No implementado"));
    }
}
