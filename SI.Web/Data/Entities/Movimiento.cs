using SI.Web.Enums;

namespace SI.Web.Data.Entities;

public sealed class Movimiento
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento Tipo { get; set; } // ENTRADA | SALIDA
    
    public decimal Cantidad { get; set; } // Cantidad debe ser mayor que cero, satisface RB - 01
    public decimal CostoUnitarioEntrada { get; set; } // Costo debe ser mayor a cero, satisface RB - 02 | Sólo entradas
    public decimal ExistenciaResultante { get; set; } // Stock Resultante
    
    public decimal CostoUnitarioPeps { get; set; } // Costo unitario asignado a PEPS
    public decimal CostoTotalPeps { get; set; } // Costo total asignado a PEPS
    public decimal SaldoValorPeps { get; set; } // Saldo patrimonial PEPS | Cuanto vale nuestro inventario
    
    public decimal CostoUnitarioPromedio { get; set; } // Costo unitario vigente
    public decimal CostoTotalPromedio { get; set; } // Costo total asignado Promedio
    public decimal SaldoValorPromedio { get; set; } // Saldo patrimonial Promedio | Cuanto vale nuestro inventario
    
    public Producto Producto { get; set; } = null!;
}