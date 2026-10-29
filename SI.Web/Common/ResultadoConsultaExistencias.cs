using SI.Web.Enums;

namespace SI.Web.Common;

public sealed record CapaPepsDto
{
    public int Id { get; init; }
    public DateTime FechaEntrada { get; init; }
    public decimal CantidadOriginal { get; init; }
    public decimal CantidadDisponible { get; init; }
    public decimal CostoUnitario { get; init; }
    public decimal Subtotal => Math.Round(CantidadDisponible * CostoUnitario, 4, MidpointRounding.AwayFromZero);
}

public sealed record ResultadoConsultaExistencias
{
    public bool Encontrado { get; init; }
    public string? MensajeError { get; init; }
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string UnidadMedida { get; init; } = string.Empty;
    public MetodoCosteo Metodo { get; init; }
    public decimal ExistenciaActual { get; init; }
    public decimal CostoUnitario { get; init; }
    public decimal ValorInventario { get; init; }
    public IReadOnlyList<CapaPepsDto> CapasActivas { get; init; } = Array.Empty<CapaPepsDto>();

    public static ResultadoConsultaExistencias NoEncontrado(string mensaje = "El producto no existe.") =>
        new()
        {
            Encontrado = false,
            MensajeError = mensaje
        };

    public static ResultadoConsultaExistencias Exitoso(
        int productoId,
        string codigo,
        string nombre,
        string unidadMedida,
        MetodoCosteo metodo,
        decimal existenciaActual,
        decimal costoUnitario,
        decimal valorInventario,
        IReadOnlyList<CapaPepsDto>? capasActivas = null) =>
        new()
        {
            Encontrado = true,
            ProductoId = productoId,
            Codigo = codigo,
            Nombre = nombre,
            UnidadMedida = unidadMedida,
            Metodo = metodo,
            ExistenciaActual = existenciaActual,
            CostoUnitario = costoUnitario,
            ValorInventario = valorInventario,
            CapasActivas = capasActivas ?? Array.Empty<CapaPepsDto>()
        };
}
