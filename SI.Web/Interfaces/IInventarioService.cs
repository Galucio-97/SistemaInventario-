using SI.Web.Common;
using SI.Web.Enums;

namespace SI.Web.Interfaces;

public interface IInventarioService
{
    Task<ResultadoOperacion> RegistrarEntradaAsync(
        int productoId, DateTime fecha, decimal cantidad, decimal costoUnitario, CancellationToken ct = default
    );

    Task<ResultadoOperacion> RegistrarSalidaAsync(
        int productoId, DateTime fecha, decimal cantidad, CancellationToken ct = default
    );

    Task<ResultadoConsultaExistencias> ConsultarExistenciasAsync(
        int productoId, MetodoCosteo metodo, CancellationToken ct = default
    );

    Task<ResultadoHistorialMovimientos> ObtenerHistorialMovimientosAsync(
        int productoId, MetodoCosteo metodo, CancellationToken ct = default
    );

    Task<IReadOnlyList<MovimientoComparativoDto>> ObtenerHistorialComparativoAsync(
        int productoId, CancellationToken ct = default
    );
}
