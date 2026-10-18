namespace SI.Web.Data.Entities;

public class Producto
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public required string UnidadMedida { get; set; }
    public bool Activo { get; set; } = true;

    public decimal ExistenciaActual { get; set; }
    public decimal CostoPromedioActual { get; set; }
    public decimal ValorInventarioPeps { get; set; }
    public decimal ValorInventarioPromedio { get; set; }
}