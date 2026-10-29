using SI.Web.Common;
using SI.Web.Interfaces;

namespace SI.Web.Services;

public class EjercicioReferenciaService(
    ProductoService productoService,
    IInventarioService inventarioService) : IEjercicioReferenciaService
{
    public Task<ResultadoCargaEjercicio> CargarEjercicioE01Async(CancellationToken ct = default)
    {
        throw new NotImplementedException("Método no implementado aún (Fase RED - SI-034).");
    }
}
