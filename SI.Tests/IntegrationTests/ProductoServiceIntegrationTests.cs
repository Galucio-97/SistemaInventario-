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

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerPorIdAsync_CuandoProductoExiste_RetornaProductoConSusDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            var res = await service.RegistrarAsync("P-010", "Tuerca M6", "pieza", "Acero inoxidable");
            productoId = res.EntidadId!.Value;
        }

        // Act
        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);
        var resultado = await serviceLectura.ObtenerPorIdAsync(productoId);

        // Assert
        Assert.True(resultado.Encontrado);
        Assert.NotNull(resultado.Producto);
        Assert.Equal("P-010", resultado.Producto.Codigo);
        Assert.Equal("Tuerca M6", resultado.Producto.Nombre);
        Assert.Equal("pieza", resultado.Producto.UnidadMedida);
        Assert.Equal("Acero inoxidable", resultado.Producto.Descripcion);
        Assert.True(resultado.Producto.Activo);
        Assert.Null(resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerPorCodigoAsync_CuandoProductoExiste_RetornaProductoConSusDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            await service.RegistrarAsync("P-020", "Rondana Plana", "pieza");
        }

        // Act
        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);
        var resultado = await serviceLectura.ObtenerPorCodigoAsync("P-020");

        // Assert
        Assert.True(resultado.Encontrado);
        Assert.NotNull(resultado.Producto);
        Assert.Equal("P-020", resultado.Producto.Codigo);
        Assert.Equal("Rondana Plana", resultado.Producto.Nombre);
        Assert.Null(resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-01")]
    [Trait("RF", "RF-05")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerPorIdAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje()
    {
        // Arrange
        fixture.ResetDatabase();
        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);

        // Act
        var resultado = await serviceLectura.ObtenerPorIdAsync(99999);

        // Assert
        Assert.False(resultado.Encontrado);
        Assert.Null(resultado.Producto);
        Assert.Equal(ProductoService.MensajeProductoNoExiste, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-01")]
    [Trait("RF", "RF-05")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerPorCodigoAsync_CuandoProductoNoExiste_RetornaNoEncontradoConMensaje()
    {
        // Arrange
        fixture.ResetDatabase();
        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);

        // Act
        var resultado = await serviceLectura.ObtenerPorCodigoAsync("NO-EXISTE-999");

        // Assert
        Assert.False(resultado.Encontrado);
        Assert.Null(resultado.Producto);
        Assert.Equal(ProductoService.MensajeProductoNoExiste, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ListarTodosAsync_RetornaTodosLosProductosConEstadoYExistencia()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            await service.RegistrarAsync("P-002", "Clavo 2 pulg", "caja");
            await service.RegistrarAsync("P-001", "Alambre galvanizado", "rollo");
        }

        // Act
        using var contextLectura = fixture.CreateDbContext();
        var serviceLectura = new ProductoService(contextLectura);
        var productos = await serviceLectura.ListarTodosAsync();

        // Assert
        Assert.Equal(2, productos.Count);
        Assert.Equal("P-001", productos[0].Codigo);
        Assert.Equal("P-002", productos[1].Codigo);
        Assert.All(productos, p =>
        {
            Assert.True(p.Activo);
            Assert.Equal(0m, p.ExistenciaActual);
        });
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-01")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task Consultar_NoModificaDatosNiCreaMovimientos()
    {
        // Arrange
        fixture.ResetDatabase();
        using (var ctx = fixture.CreateDbContext())
        {
            var service = new ProductoService(ctx);
            await service.RegistrarAsync("P-001", "Arandela", "pieza");
        }

        using var contextLectura = fixture.CreateDbContext();
        var countProductosAntes = await contextLectura.Productos.CountAsync();
        var countMovimientosAntes = await contextLectura.Movimientos.CountAsync();

        var serviceLectura = new ProductoService(contextLectura);

        // Act: Consultas existentes e inexistentes
        await serviceLectura.ObtenerPorIdAsync(1);
        await serviceLectura.ObtenerPorIdAsync(999);
        await serviceLectura.ObtenerPorCodigoAsync("P-001");
        await serviceLectura.ObtenerPorCodigoAsync("INEXISTENTE");
        await serviceLectura.ListarTodosAsync();

        // Assert: ningún estado cambió
        var countProductosDespues = await contextLectura.Productos.CountAsync();
        var countMovimientosDespues = await contextLectura.Movimientos.CountAsync();

        Assert.Equal(countProductosAntes, countProductosDespues);
        Assert.Equal(countMovimientosAntes, countMovimientosDespues);
    }
}
