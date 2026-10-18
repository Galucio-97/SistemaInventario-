using Microsoft.EntityFrameworkCore;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Tests.RegressionTests;

public class Def001PrecisionProductoTests
{
    [Fact]
    [Trait("CP", "CP-19")]
    [Trait("RF", "RB-08")]
    [Trait("DEF", "DEF-001")]
    [Trait("Nivel", "Regresion")]
    public void Producto_Schema_DebeTenerPrecisionDecimalCorrecta()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer()
            .Options;
        
        var context = new InventarioDbContext(options);
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(Producto));
        var existencia = entidad?.FindProperty(nameof(Producto.ExistenciaActual));
        var costoProm = entidad?.FindProperty(nameof(Producto.CostoPromedioActual));
        var valorPeps = entidad?.FindProperty(nameof(Producto.ValorInventarioPeps));
        var valorProm = entidad?.FindProperty(nameof(Producto.ValorInventarioPromedio));

        // Assert
        Assert.Equal(typeof(decimal), existencia?.ClrType);
        Assert.Equal(18, existencia?.GetPrecision());
        Assert.Equal(4, existencia?.GetScale());
        
        Assert.Equal(18, costoProm?.GetPrecision());
        Assert.Equal(4, costoProm?.GetScale());
        
        Assert.Equal(18, valorPeps?.GetPrecision());
        Assert.Equal(2, valorPeps?.GetScale());
        
        Assert.Equal(18, valorProm?.GetPrecision());
        Assert.Equal(2, valorProm?.GetScale());
    }
}