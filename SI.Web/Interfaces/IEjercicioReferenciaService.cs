using SI.Web.Common;

namespace SI.Web.Interfaces;

public interface IEjercicioReferenciaService
{
    public const string CodigoProductoE01 = "E01-DEMO";

    Task<ResultadoCargaEjercicio> CargarEjercicioE01Async(CancellationToken ct = default);
}
