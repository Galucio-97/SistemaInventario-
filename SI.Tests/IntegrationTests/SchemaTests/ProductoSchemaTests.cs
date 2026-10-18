using SI.Tests.Fixtures;
using SI.Web.Data.Entities;

namespace SI.Tests.IntegrationTests.SchemaTests;

public class ProductoSchemaTests(TestDbContextFixture fixture) : IClassFixture<TestDbContextFixture>
{
    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Integracion")]
    public void Producto_Schema_DebeExistirEnContexto()
    {
        // Arrange
        var context = fixture.Context;
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(Producto));
        
        // Assert
        Assert.NotNull(entidad);
    }
}