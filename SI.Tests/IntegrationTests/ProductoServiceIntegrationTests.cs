using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class ProductoServiceIntegrationTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-02")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarAsync_ProductoValido_PersisteEnBaseDeDatosConValoresPorDefecto()
    {
        // Arrange
        fixture.ResetDatabase();
        using var context = fixture.CreateDbContext();
        var service = new ProductoService(context);

        // Act
        var resultado = await service.RegistrarAsync("P-001", "Tornillo 1/4", "pieza", "");

        // Assert
        Assert.True(resultado.Exitoso);

        // Consultar con un DbContext completamente nuevo (desacoplado de cache)
        using var contextLectura = fixture.CreateDbContext();
        var guardado = await contextLectura.Productos.FirstOrDefaultAsync(p => p.Codigo == "P-001");

        Assert.NotNull(guardado);
        Assert.Equal("P-001", guardado.Codigo);
        Assert.Equal("Tornillo 1/4", guardado.Nombre);
        Assert.Equal("pieza", guardado.UnidadMedida);
        Assert.True(guardado.Activo);
        Assert.Equal(0m, guardado.ExistenciaActual);
        Assert.Equal(0.0000m, guardado.CostoPromedioActual);
        Assert.Equal(0.00m, guardado.ValorInventarioPeps);
        Assert.Equal(0.00m, guardado.ValorInventarioPromedio);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarAsync_CodigoDuplicado_RechazaYConservaUnaSolaFila()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var contextInicial = fixture.CreateDbContext())
        {
            var serviceInicial = new ProductoService(contextInicial);
            await serviceInicial.RegistrarAsync("P-001", "Tornillo 1/4", "pieza");
        }

        using var contextIntento = fixture.CreateDbContext();
        var serviceIntento = new ProductoService(contextIntento);

        // Act: intentar registrar con clave duplicada
        var resultado = await serviceIntento.RegistrarAsync("P-001", "Otro", "pieza");

        // Assert: rechazo con mensaje exacto y en BD solo queda 1 fila
        Assert.False(resultado.Exitoso);
        Assert.Equal("Ya existe un producto con el código P-001", resultado.MensajeError);

        using var contextVerificacion = fixture.CreateDbContext();
        var total = await contextVerificacion.Productos.CountAsync(p => p.Codigo == "P-001");
        Assert.Equal(1, total);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Integracion")]
    public async Task InsertarDirecto_CodigoDuplicado_LanzaDbUpdateExceptionPorIndiceUnico()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var context = fixture.CreateDbContext())
        {
            context.Productos.Add(new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza"
            });
            await context.SaveChangesAsync();
        }

        // Act & Assert: intentar insertar directo en BD con el mismo código dispara excepción por IX_Productos_Codigo
        using var contextDuplicado = fixture.CreateDbContext();
        contextDuplicado.Productos.Add(new Producto
        {
            Codigo = "P-001",
            Nombre = "Otro Tornillo",
            UnidadMedida = "pieza"
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => contextDuplicado.SaveChangesAsync());
    }
}
