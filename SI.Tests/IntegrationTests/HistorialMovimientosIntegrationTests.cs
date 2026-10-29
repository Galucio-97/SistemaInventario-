using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Services;

namespace SI.Tests.IntegrationTests;

[Collection("Database")]
public class HistorialMovimientosIntegrationTests(TestDbContextFixture fixture)
{
    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerHistorialMovimientosAsync_CuandoProductoNoExiste_RetornaNoEncontrado()
    {
        // Arrange
        fixture.ResetDatabase();
        await using var context = fixture.CreateDbContext();
        var service = new InventarioService(context);
        const int productoInexistenteId = 99999;

        // Act
        var resultado = await service.ObtenerHistorialMovimientosAsync(productoInexistenteId, MetodoCosteo.PromedioPonderado);

        // Assert
        Assert.False(resultado.Encontrado);
        Assert.NotNull(resultado.MensajeError);
        Assert.Contains("El producto no existe", resultado.MensajeError, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(resultado.Movimientos);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPromedio_CoincideFilaPorFilaConOraculo()
    {
        // Arrange — Preparar Escenario E-01 (10 @ 20, 10 @ 30, salida 5)
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-KARDEX",
                Nombre = "Producto Kardex E-01",
                UnidadMedida = "pieza",
                Activo = true
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            // Movimiento 1: Entrada 10 @ $20.00 el 2026-10-01
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m);
            // Movimiento 2: Entrada 10 @ $30.00 el 2026-10-02
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 10, 0, 0), 10.0000m, 30.0000m);
            // Movimiento 3: Salida 5 el 2026-10-03
            await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m);
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var resultado = await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.PromedioPonderado);

        // Assert — Oráculo E-01 para Promedio Ponderado
        Assert.True(resultado.Encontrado);
        Assert.Equal("PROD-KARDEX", resultado.Codigo);
        Assert.Equal(MetodoCosteo.PromedioPonderado, resultado.Metodo);
        Assert.Equal(3, resultado.Movimientos.Count);

        // Fila 1: Entrada 10 @ 20.00 -> CostoTotal 200.00, Existencia 10.0000, Saldo 200.00
        var m1 = resultado.Movimientos[0];
        Assert.Equal(TipoMovimiento.Entrada, m1.Tipo);
        Assert.Equal(10.0000m, m1.Cantidad);
        Assert.Equal(20.0000m, m1.CostoUnitario);
        Assert.Equal(200.0000m, m1.CostoTotal);
        Assert.Equal(10.0000m, m1.ExistenciaResultante);
        Assert.Equal(200.0000m, m1.SaldoValor);

        // Fila 2: Entrada 10 @ 30.00 -> CostoTotal 300.00, Existencia 20.0000, Saldo 500.00
        var m2 = resultado.Movimientos[1];
        Assert.Equal(TipoMovimiento.Entrada, m2.Tipo);
        Assert.Equal(10.0000m, m2.Cantidad);
        Assert.Equal(30.0000m, m2.CostoUnitario);
        Assert.Equal(300.0000m, m2.CostoTotal);
        Assert.Equal(20.0000m, m2.ExistenciaResultante);
        Assert.Equal(500.0000m, m2.SaldoValor);

        // Fila 3: Salida 5 -> CostoUnitario 25.00, CostoTotal 125.00, Existencia 15.0000, Saldo 375.00
        var m3 = resultado.Movimientos[2];
        Assert.Equal(TipoMovimiento.Salida, m3.Tipo);
        Assert.Equal(5.0000m, m3.Cantidad);
        Assert.Equal(25.0000m, m3.CostoUnitario);
        Assert.Equal(125.0000m, m3.CostoTotal);
        Assert.Equal(15.0000m, m3.ExistenciaResultante);
        Assert.Equal(375.0000m, m3.SaldoValor);
    }

    [Fact]
    [Trait("CP", "CP-10")]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerHistorialMovimientosAsync_EscenarioE01_MetodoPeps_CoincideFilaPorFilaConOraculo()
    {
        // Arrange — Escenario E-01
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-KARDEX",
                Nombre = "Producto Kardex E-01",
                UnidadMedida = "pieza",
                Activo = true
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            // Movimiento 1: Entrada 10 @ $20.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10.0000m, 20.0000m);
            // Movimiento 2: Entrada 10 @ $30.00
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 10, 0, 0), 10.0000m, 30.0000m);
            // Movimiento 3: Salida 5
            await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5.0000m);
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var resultado = await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.Peps);

        // Assert — Oráculo E-01 para PEPS
        Assert.True(resultado.Encontrado);
        Assert.Equal("PROD-KARDEX", resultado.Codigo);
        Assert.Equal(MetodoCosteo.Peps, resultado.Metodo);
        Assert.Equal(3, resultado.Movimientos.Count);

        // Fila 1: Entrada 10 @ 20.00 -> CostoTotal 200.00, Existencia 10.0000, Saldo PEPS 200.00
        var m1 = resultado.Movimientos[0];
        Assert.Equal(TipoMovimiento.Entrada, m1.Tipo);
        Assert.Equal(10.0000m, m1.Cantidad);
        Assert.Equal(20.0000m, m1.CostoUnitario);
        Assert.Equal(200.0000m, m1.CostoTotal);
        Assert.Equal(10.0000m, m1.ExistenciaResultante);
        Assert.Equal(200.0000m, m1.SaldoValor);

        // Fila 2: Entrada 10 @ 30.00 -> CostoTotal 300.00, Existencia 20.0000, Saldo PEPS 500.00
        var m2 = resultado.Movimientos[1];
        Assert.Equal(TipoMovimiento.Entrada, m2.Tipo);
        Assert.Equal(10.0000m, m2.Cantidad);
        Assert.Equal(30.0000m, m2.CostoUnitario);
        Assert.Equal(300.0000m, m2.CostoTotal);
        Assert.Equal(20.0000m, m2.ExistenciaResultante);
        Assert.Equal(500.0000m, m2.SaldoValor);

        // Fila 3: Salida 5 (consume Capa 1 de 10 @ 20.00) -> CostoUnitario PEPS 20.00, CostoTotal 100.00, Existencia 15.0000, Saldo PEPS 400.00
        var m3 = resultado.Movimientos[2];
        Assert.Equal(TipoMovimiento.Salida, m3.Tipo);
        Assert.Equal(5.0000m, m3.Cantidad);
        Assert.Equal(20.0000m, m3.CostoUnitario);
        Assert.Equal(100.0000m, m3.CostoTotal);
        Assert.Equal(15.0000m, m3.ExistenciaResultante);
        Assert.Equal(400.0000m, m3.SaldoValor);
    }

    [Fact]
    [Trait("RF", "RF-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerHistorialMovimientosAsync_CambioMetodo_NoAlteraFilasFechasNiCantidades()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-INV",
                Nombre = "Producto Invariante",
                UnidadMedida = "pieza",
                Activo = true
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10m, 20m);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2, 10, 0, 0), 10m, 30m);
            await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3, 10, 0, 0), 5m);
        }

        // Act
        await using var contextConsulta = fixture.CreateDbContext();
        var serviceConsulta = new InventarioService(contextConsulta);
        var historialPeps = await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.Peps);
        var historialPromedio = await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.PromedioPonderado);

        // Assert — Mismo número de filas, fechas, cantidades y existencias
        Assert.Equal(historialPeps.Movimientos.Count, historialPromedio.Movimientos.Count);

        for (int i = 0; i < historialPeps.Movimientos.Count; i++)
        {
            var pepsItem = historialPeps.Movimientos[i];
            var promItem = historialPromedio.Movimientos[i];

            Assert.Equal(pepsItem.Id, promItem.Id);
            Assert.Equal(pepsItem.Fecha, promItem.Fecha);
            Assert.Equal(pepsItem.Tipo, promItem.Tipo);
            Assert.Equal(pepsItem.Cantidad, promItem.Cantidad);
            Assert.Equal(pepsItem.ExistenciaResultante, promItem.ExistenciaResultante);
        }
    }

    [Fact]
    [Trait("RNF", "RNF-01")]
    [Trait("RB", "RB-04")]
    [Trait("RF", "RF-05")]
    [Trait("Nivel", "Integracion")]
    public async Task ObtenerHistorialMovimientosAsync_OperacionLectura_NoGeneraEfectosSecundarios()
    {
        // Arrange
        fixture.ResetDatabase();
        int productoId;

        await using (var context = fixture.CreateDbContext())
        {
            var producto = new Producto
            {
                Codigo = "PROD-RO",
                Nombre = "Producto ReadOnly",
                UnidadMedida = "pieza",
                Activo = true
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
            productoId = producto.Id;

            var service = new InventarioService(context);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1, 10, 0, 0), 10m, 20m);
        }

        int totalMovimientosAntes;
        int totalCapasAntes;

        await using (var contextSnapshot = fixture.CreateDbContext())
        {
            totalMovimientosAntes = await contextSnapshot.Movimientos.CountAsync();
            totalCapasAntes = await contextSnapshot.CapasInventarioPeps.CountAsync();
        }

        // Act — Consultar repetidamente en ambos métodos
        await using (var contextConsulta = fixture.CreateDbContext())
        {
            var serviceConsulta = new InventarioService(contextConsulta);
            await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.Peps);
            await serviceConsulta.ObtenerHistorialMovimientosAsync(productoId, MetodoCosteo.PromedioPonderado);
        }

        // Assert — La base de datos debe permanecer inalterada
        await using (var contextVerif = fixture.CreateDbContext())
        {
            var totalMovimientosDespues = await contextVerif.Movimientos.CountAsync();
            var totalCapasDespues = await contextVerif.CapasInventarioPeps.CountAsync();

            Assert.Equal(totalMovimientosAntes, totalMovimientosDespues);
            Assert.Equal(totalCapasAntes, totalCapasDespues);
        }
    }
}
