using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class ConsultaExistenciasIntegrationTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ConsultarExistenciasAsync_CuandoProductoNoExiste_RetornaNoEncontrado()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        const int productoInexistenteId = 99999;

        // Act
        var resultado = await service.ConsultarExistenciasAsync(productoInexistenteId, MetodoCosteo.Peps);

        // Assert
        Assert.False(resultado.Encontrado);
        Assert.NotNull(resultado.MensajeError);
        Assert.Contains("El producto no existe", resultado.MensajeError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ConsultarExistenciasAsync_EscenarioE01_MetodoPromedio_CalculaExistenciaYValorCorrectos()
    {
        // Arrange — Preparar Escenario E-01 (10 @ $20, 10 @ $30, salida 5)
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-001",
                Nombre = "Producto E-01",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            // Entrada 1: 10 @ $20.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m);
            // Entrada 2: 10 @ $30.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 10, 0, 0), 10.0000m, 30.0000m);
            // Salida 1: 5 unidades
            await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m);
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var resultado = await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.PromedioPonderado);

        // Assert
        Assert.True(resultado.Encontrado);
        Assert.Equal("PROD-001", resultado.Codigo);
        Assert.Equal("Producto E-01", resultado.Nombre);
        Assert.Equal(MetodoCosteo.PromedioPonderado, resultado.Metodo);
        Assert.Equal(15.0000m, resultado.ExistenciaActual);
        Assert.Equal(25.0000m, resultado.CostoUnitario);
        Assert.Equal(375.0000m, resultado.ValorInventario);
        Assert.Empty(resultado.CapasActivas);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("D", "D-09")]
    [Trait("Nivel", "Integracion")]
    public async Task ConsultarExistenciasAsync_EscenarioE01_MetodoPeps_CalculaExistenciaValorYCapasCorrectas()
    {
        // Arrange — Escenario E-01
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-001",
                Nombre = "Producto E-01",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            // Entrada 1: 10 @ $20.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m);
            // Entrada 2: 10 @ $30.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 10, 0, 0), 10.0000m, 30.0000m);
            // Salida 1: 5 unidades
            await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m);
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var resultado = await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);

        // Assert
        Assert.True(resultado.Encontrado);
        Assert.Equal("PROD-001", resultado.Codigo);
        Assert.Equal("Producto E-01", resultado.Nombre);
        Assert.Equal(MetodoCosteo.Peps, resultado.Metodo);
        Assert.Equal(15.0000m, resultado.ExistenciaActual);
        Assert.Equal(400.0000m, resultado.ValorInventario);
        // D-09: CostoUnitario PEPS = 400.0000 / 15.0000 = 26.6667 (redondeo a 4 decimales)
        Assert.Equal(26.6667m, resultado.CostoUnitario);

        // Capas PEPS activas (CantidadDisponible > 0)
        Assert.Equal(2, resultado.CapasActivas.Count);
        // Capa 1: restante 5.0000 de 10.0000 @ $20.0000
        Assert.Equal(5.0000m, resultado.CapasActivas[0].CantidadDisponible);
        Assert.Equal(10.0000m, resultado.CapasActivas[0].CantidadOriginal);
        Assert.Equal(20.0000m, resultado.CapasActivas[0].CostoUnitario);
        Assert.Equal(100.0000m, resultado.CapasActivas[0].Subtotal);

        // Capa 2: restante 10.0000 de 10.0000 @ $30.0000
        Assert.Equal(10.0000m, resultado.CapasActivas[1].CantidadDisponible);
        Assert.Equal(10.0000m, resultado.CapasActivas[1].CantidadOriginal);
        Assert.Equal(30.0000m, resultado.CapasActivas[1].CostoUnitario);
        Assert.Equal(300.0000m, resultado.CapasActivas[1].Subtotal);
    }

    [Fact]
    [Trait("D", "D-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Integracion")]
    public async Task ConsultarExistenciasAsync_MetodoPeps_CuandoExistenciaEsCero_RetornaCostoCero()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-CERO",
                Nombre = "Producto Sin Existencia",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var resultado = await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);

        // Assert
        Assert.True(resultado.Encontrado);
        Assert.Equal(0.0000m, resultado.ExistenciaActual);
        Assert.Equal(0.0000m, resultado.CostoUnitario);
        Assert.Equal(0.0000m, resultado.ValorInventario);
        Assert.Empty(resultado.CapasActivas);
    }

    [Fact]
    [Trait("RF", "RF-05")]
    [Trait("RNF", "RNF-01")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task ConsultarExistenciasAsync_NoModificaProductosMovimientosNiCapas()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-IDEM",
                Nombre = "Producto Idempotente",
                UnidadMedida = "pieza",
                Activo = true,
                ExistenciaActual = 0m,
                CostoPromedioActual = 0m,
                ValorInventarioPeps = 0m,
                ValorInventarioPromedio = 0m
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m);
        }

        // Snapshot de la base de datos antes de consultar
        int totalMovimientosAntes;
        int totalCapasAntes;
        decimal valorPepsAntes;
        decimal valorPromedioAntes;

        await using (var contextSnapshot = fixture.CreateDbContext())
        {
            totalMovimientosAntes = await contextSnapshot.Movimientos.CountAsync();
            totalCapasAntes = await contextSnapshot.CapasInventarioPeps.CountAsync();
            var prod = await contextSnapshot.Productos.FindAsync(productoId);
            valorPepsAntes = prod!.ValorInventarioPeps;
            valorPromedioAntes = prod.ValorInventarioPromedio;
        }

        // Act — Consultar repetidamente en ambos métodos
        await using (var contextConsulta = fixture.CreateDbContext())
        {
            var serviceConsulta = new InventarioService(contextConsulta);
            await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);
            await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.PromedioPonderado);
            await serviceConsulta.ConsultarExistenciasAsync(productoId, MetodoCosteo.Peps);
        }

        // Assert — La base de datos debe ser exactamente idéntica
        await using (var contextVerif = fixture.CreateDbContext())
        {
            var totalMovimientosDespues = await contextVerif.Movimientos.CountAsync();
            var totalCapasDespues = await contextVerif.CapasInventarioPeps.CountAsync();
            var prodDespues = await contextVerif.Productos.FindAsync(productoId);

            Assert.Equal(totalMovimientosAntes, totalMovimientosDespues);
            Assert.Equal(totalCapasAntes, totalCapasDespues);
            Assert.Equal(valorPepsAntes, prodDespues!.ValorInventarioPeps);
            Assert.Equal(valorPromedioAntes, prodDespues.ValorInventarioPromedio);
        }
    }
}
