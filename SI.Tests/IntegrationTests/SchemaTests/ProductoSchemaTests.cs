using Microsoft.EntityFrameworkCore;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Tests.IntegrationTests.SchemaTests;

public class ProductoSchemaTests
{
    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Integracion")]
    public void Producto_Schema_DebeExistirEnContexto()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer()
            .Options;
        
        var context = new InventarioDbContext(options);
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(Producto));
        
        // Assert
        Assert.NotNull(entidad);
    }
}