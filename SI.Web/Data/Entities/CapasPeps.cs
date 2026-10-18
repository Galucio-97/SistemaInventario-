namespace SI.Web.Data.Entities;

public sealed class CapasPeps
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int MovimientoId { get; set; }
    
    public DateTime FechaEntrada { get; set; } // Orden FIFO (Fecha ASC, Id ASC)
    public decimal CantidadInicial { get; set; } // Lote original adquirido
    public decimal CantidadDisponible { get; set; } // Saldo restante de la capa
    public decimal CostoUnitario { get; set; } // Coste de compra del lote

    public Producto Producto { get; set; } = null!;
    public Movimiento Movimiento { get; set; } = null!;
}