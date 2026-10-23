using SI.Web.Common;

namespace SI.Web.Interfaces;

public interface IInventarioService
{
    Task<ResultadoOperacion> RegistrarEntradaAsync(
        int productoId, DateTime fecha, decimal cantidad, decimal costoUnitario, CancellationToken ct = default
    );
}