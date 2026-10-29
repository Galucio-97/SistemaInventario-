using SI.Web.Enums;

namespace SI.Web.Common;

public sealed record MovimientoHistorialDto
{
    public int Id { get; init; }
    public DateTime Fecha { get; init; }
    public TipoMovimiento Tipo { get; init; }
    public decimal Cantidad { get; init; }
    public decimal CostoUnitario { get; init; }
    public decimal CostoTotal { get; init; }
    public decimal ExistenciaResultante { get; init; }
    public decimal SaldoValor { get; init; }
}

public sealed record ResultadoHistorialMovimientos
{
    public bool Encontrado { get; init; }
    public string? MensajeError { get; init; }
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public MetodoCosteo Metodo { get; init; }
    public IReadOnlyList<MovimientoHistorialDto> Movimientos { get; init; } = Array.Empty<MovimientoHistorialDto>();

    public static ResultadoHistorialMovimientos NoEncontrado(string mensaje = "El producto no existe.") =>
        new()
        {
            Encontrado = false,
            MensajeError = mensaje
        };

    public static ResultadoHistorialMovimientos Exitoso(
        int productoId,
        string codigo,
        string nombre,
        MetodoCosteo metodo,
        IReadOnlyList<MovimientoHistorialDto> movimientos) =>
        new()
        {
            Encontrado = true,
            ProductoId = productoId,
            Codigo = codigo,
            Nombre = nombre,
            Metodo = metodo,
            Movimientos = movimientos
        };
}
