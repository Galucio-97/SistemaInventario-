namespace SI.Web.Common;

public sealed record ResultadoCargaEjercicio
{
    public bool Exitoso { get; init; }
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public bool YaExistia { get; init; }
    public string? Mensaje { get; init; }

    public static ResultadoCargaEjercicio Creado(int productoId, string codigo, string mensaje) =>
        new()
        {
            Exitoso = true,
            ProductoId = productoId,
            Codigo = codigo,
            YaExistia = false,
            Mensaje = mensaje
        };

    public static ResultadoCargaEjercicio Existente(int productoId, string codigo, string mensaje) =>
        new()
        {
            Exitoso = true,
            ProductoId = productoId,
            Codigo = codigo,
            YaExistia = true,
            Mensaje = mensaje
        };

    public static ResultadoCargaEjercicio Fallido(string mensaje) =>
        new()
        {
            Exitoso = false,
            Mensaje = mensaje
        };
}
