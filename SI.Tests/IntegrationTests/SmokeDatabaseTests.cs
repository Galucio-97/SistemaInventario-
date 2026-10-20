using SI.Tests.Fixtures;
using SI.Web.Data.Entities;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class SmokeDatabaseTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("Nivel", "Integracion")]
    [Trait("CP", "CP-01")]
    public void InsertarYLeerProducto_ConContextosNuevos_PersisteEnBaseDeDatosReal()
    {
        // Arrange - Limpiar BD para iniciar desde un estado conocido (sin datos residuales)
        fixture.ResetDatabase();

        var producto = new Producto
        {
            Codigo = "PROD-SMOKE-01",
            Nombre = "Tornillo 1/4 Acero",
            Descripcion = "Prueba de humo de persistencia en SQL Server",
            UnidadMedida = "Pieza",
            Activo = true,
            ExistenciaActual = 15.0000m,
            CostoPromedioActual = 20.0000m,
            ValorInventarioPeps = 300.00m,
            ValorInventarioPromedio = 300.00m
        };

        // Act - Guardar usando una primera instancia de DbContext
        using (var contextEscritura = fixture.CreateDbContext())
        {
            contextEscritura.Productos.Add(producto);
            contextEscritura.SaveChanges();
        }

        // Assert - Recuperar usando una instancia de DbContext completamente nueva (sin cache local de EF)
        using (var contextLectura = fixture.CreateDbContext())
        {
            var productoRecuperado = contextLectura.Productos.FirstOrDefault(p => p.Codigo == "PROD-SMOKE-01");

            Assert.NotNull(productoRecuperado);
            Assert.True(productoRecuperado.Id > 0);
            Assert.Equal("PROD-SMOKE-01", productoRecuperado.Codigo);
            Assert.Equal("Tornillo 1/4 Acero", productoRecuperado.Nombre);
            Assert.Equal("Pieza", productoRecuperado.UnidadMedida);
            Assert.True(productoRecuperado.Activo);
            Assert.Equal(15.0000m, productoRecuperado.ExistenciaActual);
            Assert.Equal(20.0000m, productoRecuperado.CostoPromedioActual);
            Assert.Equal(300.00m, productoRecuperado.ValorInventarioPeps);
            Assert.Equal(300.00m, productoRecuperado.ValorInventarioPromedio);
        }
    }
}
