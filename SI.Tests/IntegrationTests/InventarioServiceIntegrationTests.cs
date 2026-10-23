using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class InventarioServiceIntegrationTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-03")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-04")]
    [Trait("RB", "RB-05")]
    [Trait("RNF", "RNF-04")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_EntradaValida_CreaMovimientoCapaYActualizaProductoEnTransaccion()
    {
        // Arrange — Producto P-001 activo sin movimientos (CP-03, E-01 E1)
        fixture.ResetDatabase();
        int productoId;
        using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            productoId = p.Id;
        }

        using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var fechaEntrada = new DateTime(2026, 10, 1, 10, 0, 0);
        decimal cantidad = 10.0000m;
        decimal costoUnitario = 20.0000m;

        // Act — CP-03: Entrada de 10 @ 20.00
        var resultado = await service.RegistrarEntradaAsync(productoId, fechaEntrada, cantidad, costoUnitario);

        // Assert — Operación exitosa
        Assert.True(resultado.Exitoso);
        Assert.NotNull(resultado.EntidadId);

        // Verificación con contexto desacoplado de lectura
        using var contextLectura = fixture.CreateDbContext();
        var productoActualizado = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoActualizado);
        Assert.Equal(10.0000m, productoActualizado.ExistenciaActual);
        Assert.Equal(20.0000m, productoActualizado.CostoPromedioActual);
        Assert.Equal(200.00m, productoActualizado.ValorInventarioPeps);
        Assert.Equal(200.00m, productoActualizado.ValorInventarioPromedio);

        // Movimiento registrado en BD
        var movimiento = await contextLectura.Movimientos.FirstOrDefaultAsync(m => m.ProductoId == productoId);
        Assert.NotNull(movimiento);
        Assert.Equal(resultado.EntidadId, movimiento.Id);
        Assert.Equal(TipoMovimiento.Entrada, movimiento.Tipo);
        Assert.Equal(fechaEntrada, movimiento.Fecha);
        Assert.Equal(10.0000m, movimiento.Cantidad);
        Assert.Equal(20.0000m, movimiento.CostoUnitarioEntrada);
        Assert.Equal(10.0000m, movimiento.ExistenciaResultante);
        Assert.Equal(20.0000m, movimiento.CostoUnitarioPeps);
        Assert.Equal(200.00m, movimiento.CostoTotalPeps);
        Assert.Equal(200.00m, movimiento.SaldoValorPeps);
        Assert.Equal(20.0000m, movimiento.CostoUnitarioPromedio);
        Assert.Equal(200.00m, movimiento.CostoTotalPromedio);
        Assert.Equal(200.00m, movimiento.SaldoValorPromedio);

        // Capa PEPS registrada en BD
        var capa = await contextLectura.CapasInventarioPeps.FirstOrDefaultAsync(c => c.ProductoId == productoId);
        Assert.NotNull(capa);
        Assert.Equal(movimiento.Id, capa.MovimientoId);
        Assert.Equal(fechaEntrada, capa.FechaEntrada);
        Assert.Equal(10.0000m, capa.CantidadInicial);
        Assert.Equal(10.0000m, capa.CantidadDisponible);
        Assert.Equal(20.0000m, capa.CostoUnitario);
    }

    [Theory]
    [InlineData(-1, "La cantidad debe ser mayor que cero.")]
    [InlineData(0, "La cantidad debe ser mayor que cero.")]
    [InlineData(10.12345, "La cantidad no puede tener más de 4 decimales.")]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-01")]
    [Trait("RNF", "RNF-04")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos(
        decimal cantidadInvalida,
        string mensajeEsperado)
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza",
                Activo = true
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            productoId = p.Id;
        }

        using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);

        // Act
        var resultado = await service.RegistrarEntradaAsync(productoId, DateTime.Now, cantidadInvalida, 20.0000m);

        // Assert — Rechazado con mensaje de RB-01
        Assert.False(resultado.Exitoso);
        Assert.Equal(mensajeEsperado, resultado.MensajeError);

        // Base de datos completamente inalterada
        using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
        Assert.Empty(contextLectura.CapasInventarioPeps);

        var productoGuardado = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoGuardado);
        Assert.Equal(0m, productoGuardado.ExistenciaActual);
        Assert.Equal(0m, productoGuardado.ValorInventarioPeps);
        Assert.Equal(0m, productoGuardado.ValorInventarioPromedio);
    }

    [Theory]
    [InlineData(-5, "El costo unitario debe ser mayor que cero.")]
    [InlineData(0, "El costo unitario debe ser mayor que cero.")]
    [InlineData(20.12345, "El costo unitario no puede tener más de 4 decimales.")]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-02")]
    [Trait("RNF", "RNF-04")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoCostoUnitarioEsInvalido_RechazaYNoModificaBaseDeDatos(
        decimal costoInvalido,
        string mensajeEsperado)
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;
        using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza",
                Activo = true
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            productoId = p.Id;
        }

        using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);

        // Act
        var resultado = await service.RegistrarEntradaAsync(productoId, DateTime.Now, 10.0000m, costoInvalido);

        // Assert — Rechazado con mensaje de RB-02
        Assert.False(resultado.Exitoso);
        Assert.Equal(mensajeEsperado, resultado.MensajeError);

        // Base de datos inalterada
        using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
        Assert.Empty(contextLectura.CapasInventarioPeps);

        var productoGuardado = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoGuardado);
        Assert.Equal(0m, productoGuardado.ExistenciaActual);
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoProductoNoExiste_RechazaConMensajeYNoModificaBaseDeDatos()
    {
        // Arrange
        fixture.ResetDatabase();
        using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);

        // Act — Intentar registrar entrada sobre producto inexistente
        var resultado = await service.RegistrarEntradaAsync(99999, DateTime.Now, 10.0000m, 20.0000m);

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(InventarioService.MensajeProductoNoExiste, resultado.MensajeError);

        using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
        Assert.Empty(contextLectura.CapasInventarioPeps);
    }

    [Fact]
    [Trait("CP", "CP-18")]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-10")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoProductoEstaInactivo_RechazaConMensajeYNoModificaBaseDeDatos()
    {
        // Arrange — Producto P-002 inactivo (CP-18)
        fixture.ResetDatabase();
        int productoId;
        using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-002",
                Nombre = "Clavo 2 pulg",
                UnidadMedida = "pieza",
                Activo = false
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            productoId = p.Id;
        }

        using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);

        // Act
        var resultado = await service.RegistrarEntradaAsync(productoId, DateTime.Now, 5.0000m, 10.0000m);

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(InventarioService.MensajeProductoInactivo, resultado.MensajeError);

        // Base de datos sin cambios
        using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
        Assert.Empty(contextLectura.CapasInventarioPeps);

        var productoGuardado = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoGuardado);
        Assert.Equal(0m, productoGuardado.ExistenciaActual);
    }

    [Fact]
    [Trait("RF", "RF-02")]
    [Trait("RB", "RB-04")]
    [Trait("RB", "RB-05")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_MultiplesEntradasConsecutivas_AcumulaExistenciasCapasYActualizaPromedioPonderado()
    {
        // Arrange — Escenario E-01: E1 (10 @ 20.00) seguido de E2 (10 @ 30.00)
        fixture.ResetDatabase();
        int productoId;
        using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-001",
                Nombre = "Tornillo 1/4",
                UnidadMedida = "pieza",
                Activo = true
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            productoId = p.Id;
        }

        using (var contextE1 = fixture.CreateDbContext())
        {
            var service = new InventarioService(contextE1);
            var resE1 = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), 10.0000m, 20.0000m);
            Assert.True(resE1.Exitoso);
        }

        // Act — Registrar E2 (10 @ 30.00)
        using (var contextE2 = fixture.CreateDbContext())
        {
            var service = new InventarioService(contextE2);
            var resE2 = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2), 10.0000m, 30.0000m);
            Assert.True(resE2.Exitoso);
        }

        // Assert — Estado acumulado E-01 tras E1 + E2:
        // Existencia = 20.0000, Promedio = 25.0000, ValorPromedio = 500.00, ValorPEPS = 500.00
        using var contextLectura = fixture.CreateDbContext();
        var productoFinal = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoFinal);
        Assert.Equal(20.0000m, productoFinal.ExistenciaActual);
        Assert.Equal(25.0000m, productoFinal.CostoPromedioActual);
        Assert.Equal(500.00m, productoFinal.ValorInventarioPromedio);
        Assert.Equal(500.00m, productoFinal.ValorInventarioPeps);

        Assert.Equal(2, await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId));
        Assert.Equal(2, await contextLectura.CapasInventarioPeps.CountAsync(c => c.ProductoId == productoId));
    }
}
