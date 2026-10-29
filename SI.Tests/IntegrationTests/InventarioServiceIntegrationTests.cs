using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Common;
using SI.Web.Data.Entities;
using SI.Web.Enums;
using SI.Web.Interfaces;
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

        await using (var contextInicial = fixture.CreateDbContext())
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

        await using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var fechaEntrada = new DateTime(2026, 10, 1, 10, 0, 0);
        var cantidad = 10.0000m;
        var costoUnitario = 20.0000m;

        // Act — CP-03: Entrada de 10 @ 20.00
        var resultado = await service.RegistrarEntradaAsync(productoId, fechaEntrada, cantidad, costoUnitario);

        // Assert — Operación exitosa
        Assert.True(resultado.Exitoso);
        Assert.NotNull(resultado.EntidadId);

        // Verificación con contexto desacoplado de lectura
        await using var contextLectura = fixture.CreateDbContext();
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
        await using var contextOperacion = fixture.CreateDbContext();
        
        var service = new InventarioService(contextOperacion);

        // Act — Intentar registrar entrada sobre producto inexistente
        var resultado = await service.RegistrarEntradaAsync(99999, DateTime.Now, 10.0000m, 20.0000m);

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(InventarioService.MensajeProductoNoExiste, resultado.MensajeError);

        await using var contextLectura = fixture.CreateDbContext();
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

    [Fact]
    [Trait("CP", "CP-04")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_SalidaValidaEscenarioE01_S1_ConsumeCapasCalculaCostosAmbosMetodosYActualizaProductoEnTransaccion()
    {
        // Arrange — Precondición E-01: Producto P-001 con E1 (10 @ 20.00) y E2 (10 @ 30.00)
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
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

        // Registrar E1 (10 @ 20.00)
        await using (var contextE1 = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(contextE1);
            var resE1 = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), 10.0000m, 20.0000m);
            Assert.True(resE1.Exitoso);
        }

        // Registrar E2 (10 @ 30.00)
        await using (var contextE2 = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(contextE2);
            var resE2 = await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2), 10.0000m, 30.0000m);
            Assert.True(resE2.Exitoso);
        }

        // Act — CP-04: Registrar Salida S1 de 5 unidades en 2026-10-03
        var fechaSalida = new DateTime(2026, 10, 3, 11, 0, 0);
        decimal cantidadSalida = 5.0000m;
        ResultadoOperacion resultadoS1;

        await using (var contextS1 = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(contextS1);
            resultadoS1 = await service.RegistrarSalidaAsync(productoId, fechaSalida, cantidadSalida);
        }

        // Assert — S1 exitosa con Id de movimiento generado
        Assert.True(resultadoS1.Exitoso);
        Assert.NotNull(resultadoS1.EntidadId);

        // Verificación con contexto de lectura desacoplado (Oráculo E-01 S1)
        await using var contextLectura = fixture.CreateDbContext();
        var productoActualizado = await contextLectura.Productos.FindAsync(productoId);
        
        Assert.NotNull(productoActualizado);
        Assert.Equal(15.0000m, productoActualizado.ExistenciaActual);
        Assert.Equal(25.0000m, productoActualizado.CostoPromedioActual);
        Assert.Equal(400.00m, productoActualizado.ValorInventarioPeps);
        Assert.Equal(375.00m, productoActualizado.ValorInventarioPromedio);

        // Movimiento S1 registrado en BD
        var movimientoSalida = await contextLectura.Movimientos.FindAsync(resultadoS1.EntidadId);
        
        Assert.NotNull(movimientoSalida);
        Assert.Equal(TipoMovimiento.Salida, movimientoSalida.Tipo);
        Assert.Equal(fechaSalida, movimientoSalida.Fecha);
        Assert.Equal(5.0000m, movimientoSalida.Cantidad);
        Assert.Equal(15.0000m, movimientoSalida.ExistenciaResultante);

        // Costos PEPS en movimiento: 5 @ 20.00 = 100.00, saldo remanente 400.00
        Assert.Equal(20.0000m, movimientoSalida.CostoUnitarioPeps);
        Assert.Equal(100.00m, movimientoSalida.CostoTotalPeps);
        Assert.Equal(400.00m, movimientoSalida.SaldoValorPeps);

        // Costos Promedio en movimiento: 5 @ 25.00 = 125.00, saldo remanente 375.00
        Assert.Equal(25.0000m, movimientoSalida.CostoUnitarioPromedio);
        Assert.Equal(125.00m, movimientoSalida.CostoTotalPromedio);
        Assert.Equal(375.00m, movimientoSalida.SaldoValorPromedio);

        // Capas resultantes: Capa 1 consumida parcialmente (quedan 5), Capa 2 intacta (quedan 10)
        var capas = await contextLectura.CapasInventarioPeps
            .Where(c => c.ProductoId == productoId)
            .OrderBy(c => c.FechaEntrada)
            .ThenBy(c => c.Id)
            .ToListAsync();

        Assert.Equal(2, capas.Count);
        Assert.Equal(5.0000m, capas[0].CantidadDisponible); // 10 - 5 = 5
        Assert.Equal(10.0000m, capas[1].CantidadDisponible); // 10 intactos
    }

    [Fact]
    [Trait("CP", "CP-16")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-03")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_SalidaIgualATodaLaExistencia_AceptaDejaExistenciaCeroYValoresEnCero()
    {
        // Arrange — Producto con E1 (10 @ 20.00) y E2 (10 @ 30.00), total 20 unidades
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
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

        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(context);
            await s.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), 10.0000m, 20.0000m);
            await s.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2), 10.0000m, 30.0000m);
        }

        // Salida previa S1 de 5 unidades, dejando 15 unidades disponibles
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(context);
            await s.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3), 5.0000m);
        }

        // Act — CP-16: Salida de exactamente las 15 unidades restantes
        ResultadoOperacion resultadoS2;
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(context);
            resultadoS2 = await s.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 4), 15.0000m);
        }

        // Assert — Salida aceptada (existencia 0, valor 0.00 en ambos métodos)
        Assert.True(resultadoS2.Exitoso);

        await using var contextLectura = fixture.CreateDbContext();
        var productoFinal = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(productoFinal);
        Assert.Equal(0.0000m, productoFinal.ExistenciaActual);
        Assert.Equal(0.00m, productoFinal.ValorInventarioPeps);
        Assert.Equal(0.00m, productoFinal.ValorInventarioPromedio);

        // Movimiento de salida con saldo restante 0.00
        var mov = await contextLectura.Movimientos.FindAsync(resultadoS2.EntidadId);
        Assert.NotNull(mov);
        Assert.Equal(0.0000m, mov.ExistenciaResultante);
        Assert.Equal(400.00m, mov.CostoTotalPeps); // 5 @ 20 + 10 @ 30 = 400.00
        Assert.Equal(0.00m, mov.SaldoValorPeps);
        Assert.Equal(375.00m, mov.CostoTotalPromedio); // 15 @ 25 = 375.00
        Assert.Equal(0.00m, mov.SaldoValorPromedio);

        // Todas las capas en 0 disponible
        var capasDisponibles = await contextLectura.CapasInventarioPeps
            .Where(c => c.ProductoId == productoId && c.CantidadDisponible > 0)
            .ToListAsync();
        Assert.Empty(capasDisponibles);
    }

    [Theory]
    [InlineData(15.0001, 15.0000)]
    [InlineData(20.0000, 15.0000)]
    [Trait("CP", "CP-07")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-03")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoExistenciaEsInsuficiente_RechazaYNoModificaBaseDeDatos(
        decimal cantidadSalida,
        decimal existenciaDisponible)
    {
        // Arrange — Producto con 15 unidades de existencia
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
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

        await using (var contextEntrada = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(contextEntrada);
            await s.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), existenciaDisponible, 20.0000m);
        }

        // Act — Intentar salida que excede la existencia
        ResultadoOperacion resultado;
        await using (var contextSalida = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(contextSalida);
            resultado = await s.RegistrarSalidaAsync(productoId, DateTime.Now, cantidadSalida);
        }

        // Assert — Rechazado con mensaje de RB-03
        Assert.False(resultado.Exitoso);
        Assert.Contains("Existencia insuficiente", resultado.MensajeError);

        // Base de datos inalterada (solo 1 movimiento: la entrada previa)
        await using var contextLectura = fixture.CreateDbContext();
        Assert.Equal(1, await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId));
        Assert.False(await contextLectura.Movimientos.AnyAsync(m => m.Tipo == TipoMovimiento.Salida));

        var prod = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(prod);
        Assert.Equal(existenciaDisponible, prod.ExistenciaActual);
    }

    [Theory]
    [InlineData(-1, "La cantidad debe ser mayor que cero.")]
    [InlineData(0, "La cantidad debe ser mayor que cero.")]
    [InlineData(5.12345, "La cantidad no puede tener más de 4 decimales.")]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-01")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoCantidadEsInvalida_RechazaYNoModificaBaseDeDatos(
        decimal cantidadInvalida,
        string mensajeEsperado)
    {
        // Arrange — Producto con 10 unidades
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
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

        await using (var contextEntrada = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(contextEntrada);
            await s.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), 10.0000m, 20.0000m);
        }

        // Act
        ResultadoOperacion resultado;
        await using (var contextSalida = fixture.CreateDbContext())
        {
            IInventarioService s = new InventarioService(contextSalida);
            resultado = await s.RegistrarSalidaAsync(productoId, DateTime.Now, cantidadInvalida);
        }

        // Assert — Rechazado con mensaje de RB-01
        Assert.False(resultado.Exitoso);
        Assert.Equal(mensajeEsperado, resultado.MensajeError);

        // Sin movimientos de salida creados
        await using var contextLectura = fixture.CreateDbContext();
        Assert.False(await contextLectura.Movimientos.AnyAsync(m => m.Tipo == TipoMovimiento.Salida));
    }

    [Fact]
    [Trait("CP", "CP-11")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-04")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoProductoNoExiste_RechazaConMensajeYNoModificaBaseDeDatos()
    {
        // Arrange
        fixture.ResetDatabase();

        // Act
        ResultadoOperacion resultado;
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            resultado = await service.RegistrarSalidaAsync(99999, DateTime.Now, 5.0000m);
        }

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(InventarioService.MensajeProductoNoExiste, resultado.MensajeError);

        await using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
    }

    [Fact]
    [Trait("CP", "CP-18")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-10")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoProductoEstaInactivo_RechazaConMensajeYNoModificaBaseDeDatos()
    {
        // Arrange — Producto inactivo (CP-18)
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
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

        // Act
        ResultadoOperacion resultado;
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            resultado = await service.RegistrarSalidaAsync(productoId, DateTime.Now, 5.0000m);
        }

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(InventarioService.MensajeProductoInactivo, resultado.MensajeError);

        await using var contextLectura = fixture.CreateDbContext();
        Assert.Empty(contextLectura.Movimientos);
    }

    [Fact]
    [Trait("CP", "CP-15")]
    [Trait("RF", "RF-03")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_EscenarioE02_SalidaMulticapa_ConsumeCapasEnOrdenYCalculaCostos()
    {
        // Arrange — Escenario E-02: E1 (3 @ 10.00) y E2 (4 @ 11.00)
        fixture.ResetDatabase();
        int productoId;

        await using (var contextInicial = fixture.CreateDbContext())
        {
            var p = new Producto
            {
                Codigo = "P-002",
                Nombre = "Artículo E-02",
                UnidadMedida = "pieza",
                Activo = true
            };
            contextInicial.Productos.Add(p);
            await contextInicial.SaveChangesAsync();
            
            productoId = p.Id;
        }

        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 1), 3.0000m, 10.0000m);
            await service.RegistrarEntradaAsync(productoId, new DateTime(2026, 10, 2), 4.0000m, 11.0000m);
        }

        // Act — CP-15: Salida S1 de 5 unidades (3 @ 10.00 + 2 @ 11.00 = 52.00)
        ResultadoOperacion resultadoS1;
        await using (var context = fixture.CreateDbContext())
        {
            IInventarioService service = new InventarioService(context);
            resultadoS1 = await service.RegistrarSalidaAsync(productoId, new DateTime(2026, 10, 3), 5.0000m);
        }

        // Assert — Oráculo E-02 S1:
        // Existencia = 2, Costo PEPS = 52.00, Saldo PEPS = 22.00, Costo Prom = 52.86, Saldo Prom = 21.14
        Assert.True(resultadoS1.Exitoso);

        await using var contextLectura = fixture.CreateDbContext();
        var prod = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(prod);
        Assert.Equal(2.0000m, prod.ExistenciaActual);
        Assert.Equal(22.00m, prod.ValorInventarioPeps);
        Assert.Equal(21.14m, prod.ValorInventarioPromedio);

        var movS1 = await contextLectura.Movimientos.FindAsync(resultadoS1.EntidadId);
        Assert.NotNull(movS1);
        Assert.Equal(52.00m, movS1.CostoTotalPeps);
        Assert.Equal(22.00m, movS1.SaldoValorPeps);
        Assert.Equal(52.86m, movS1.CostoTotalPromedio);
        Assert.Equal(21.14m, movS1.SaldoValorPromedio);

        // Capa 1 agotada (0 disponibles), Capa 2 con 2 disponibles
        var capas = await contextLectura.CapasInventarioPeps
            .Where(c => c.ProductoId == productoId)
            .OrderBy(c => c.FechaEntrada)
            .ThenBy(c => c.Id)
            .ToListAsync();

        Assert.Equal(2, capas.Count);
        Assert.Equal(0.0000m, capas[0].CantidadDisponible);
        Assert.Equal(2.0000m, capas[1].CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos()
    {
        // Arrange — Producto con un movimiento inicial registrado el 2026-10-10
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
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
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;

            IInventarioService inventarioService = new InventarioService(contextSetup);
            var res = await inventarioService.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 10, 12, 0, 0), 10.0000m, 20.0000m);
            Assert.True(res.Exitoso);
        }

        // Act — Intentar registrar entrada retroactiva (2026-10-09)
        await using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var fechaRetroactiva = new DateTime(2026, 10, 9, 12, 0, 0);

        var resultado = await service.RegistrarEntradaAsync(productoId, fechaRetroactiva, 5.0000m, 25.0000m);

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(ValidadorFechaMovimiento.MensajeFechaRetroactiva, resultado.MensajeError);

        // Verificación con contexto desacoplado de lectura: la BD no fue modificada
        await using var contextLectura = fixture.CreateDbContext();
        var totalMovimientos = await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(1, totalMovimientos);

        var producto = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(producto);
        Assert.Equal(10.0000m, producto.ExistenciaActual);
        Assert.Equal(200.00m, producto.ValorInventarioPeps);
        Assert.Equal(200.00m, producto.ValorInventarioPromedio);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarEntradaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente()
    {
        // Arrange — Producto con un movimiento inicial registrado el 2026-10-10 12:00:00
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
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
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;

            IInventarioService inventarioService = new InventarioService(contextSetup);
            var res = await inventarioService.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 10, 12, 0, 0), 10.0000m, 20.0000m);
            Assert.True(res.Exitoso);
        }

        // Act — Registrar entrada con la misma fecha (2026-10-10 12:00:00)
        await using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var mismaFecha = new DateTime(2026, 10, 10, 12, 0, 0);

        var resultado = await service.RegistrarEntradaAsync(productoId, mismaFecha, 5.0000m, 20.0000m);

        // Assert
        Assert.True(resultado.Exitoso);
        Assert.NotNull(resultado.EntidadId);

        await using var contextLectura = fixture.CreateDbContext();
        var totalMovimientos = await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(2, totalMovimientos);

        var producto = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(producto);
        Assert.Equal(15.0000m, producto.ExistenciaActual);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoFechaEsAnteriorAUltimoMovimiento_RechazaYNoModificaBaseDeDatos()
    {
        // Arrange — Producto con entrada el 2026-10-10 12:00:00
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
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
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;

            IInventarioService inventarioService = new InventarioService(contextSetup);
            var res = await inventarioService.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 10, 12, 0, 0), 10.0000m, 20.0000m);
            Assert.True(res.Exitoso);
        }

        // Act — Intentar registrar salida retroactiva (2026-10-08)
        await using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var fechaRetroactiva = new DateTime(2026, 10, 8, 10, 0, 0);

        var resultado = await service.RegistrarSalidaAsync(productoId, fechaRetroactiva, 3.0000m);

        // Assert
        Assert.False(resultado.Exitoso);
        Assert.Equal(ValidadorFechaMovimiento.MensajeFechaRetroactiva, resultado.MensajeError);

        // Verificación con contexto desacoplado de lectura
        await using var contextLectura = fixture.CreateDbContext();
        var totalMovimientos = await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(1, totalMovimientos);

        var producto = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(producto);
        Assert.Equal(10.0000m, producto.ExistenciaActual);
        Assert.Equal(200.00m, producto.ValorInventarioPeps);

        var capa = await contextLectura.CapasInventarioPeps.FirstAsync(c => c.ProductoId == productoId);
        Assert.Equal(10.0000m, capa.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Integracion")]
    public async Task RegistrarSalidaAsync_CuandoFechaEsIgualAUltimoMovimiento_AceptaYRegistraCorrectamente()
    {
        // Arrange — Producto con entrada el 2026-10-10 12:00:00
        fixture.ResetDatabase();
        int productoId;

        await using (var contextSetup = fixture.CreateDbContext())
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
            contextSetup.Productos.Add(p);
            await contextSetup.SaveChangesAsync();
            productoId = p.Id;

            IInventarioService inventarioService = new InventarioService(contextSetup);
            var res = await inventarioService.RegistrarEntradaAsync(
                productoId, new DateTime(2026, 10, 10, 12, 0, 0), 10.0000m, 20.0000m);
            Assert.True(res.Exitoso);
        }

        // Act — Registrar salida con la misma fecha (2026-10-10 12:00:00)
        await using var contextOperacion = fixture.CreateDbContext();
        var service = new InventarioService(contextOperacion);
        var mismaFecha = new DateTime(2026, 10, 10, 12, 0, 0);

        var resultado = await service.RegistrarSalidaAsync(productoId, mismaFecha, 3.0000m);

        // Assert
        Assert.True(resultado.Exitoso);
        Assert.NotNull(resultado.EntidadId);

        await using var contextLectura = fixture.CreateDbContext();
        var totalMovimientos = await contextLectura.Movimientos.CountAsync(m => m.ProductoId == productoId);
        Assert.Equal(2, totalMovimientos);

        var producto = await contextLectura.Productos.FindAsync(productoId);
        Assert.NotNull(producto);
        Assert.Equal(7.0000m, producto.ExistenciaActual);

        var capa = await contextLectura.CapasInventarioPeps.FirstAsync(c => c.ProductoId == productoId);
        Assert.Equal(7.0000m, capa.CantidadDisponible);
    }
}
