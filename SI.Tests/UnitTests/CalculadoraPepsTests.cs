using SI.Web.Data.Entities;
using SI.Web.Services;

namespace SI.Tests.UnitTests;

public class CalculadoraPepsTests
{
    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CrearCapa_ConDatosEntrada_DebeInicializarCantidadDisponibleIgualAInicial()
    {
        // Arrange
        var fecha = new DateTime(2026, 10, 1, 10, 0, 0);
        decimal cantidad = 10.0000m;
        decimal costoUnitario = 20.0000m;

        // Act
        var capa = CalculadoraPeps.CrearCapa(fecha, cantidad, costoUnitario, productoId: 1, movimientoId: 100);

        // Assert
        Assert.Equal(fecha, capa.FechaEntrada);
        Assert.Equal(10.0000m, capa.CantidadInicial);
        Assert.Equal(10.0000m, capa.CantidadDisponible);
        Assert.Equal(20.0000m, capa.CostoUnitario);
        Assert.Equal(1, capa.ProductoId);
        Assert.Equal(100, capa.MovimientoId);
    }

    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularValorInventario_ConMultiplesCapas_DebeCalcularSumaDeValoresRedondeadaComoImporte()
    {
        // Arrange — E1 (10 @ 20.00) y E2 (10 @ 30.00)
        var capas = new List<CapasPeps>
        {
            new() { Id = 1, FechaEntrada = new DateTime(2026, 10, 1), CantidadInicial = 10m, CantidadDisponible = 10m, CostoUnitario = 20.0000m },
            new() { Id = 2, FechaEntrada = new DateTime(2026, 10, 2), CantidadInicial = 10m, CantidadDisponible = 10m, CostoUnitario = 30.0000m }
        };

        // Act
        var valorTotal = CalculadoraPeps.CalcularValorInventario(capas);

        // Assert — 10*20 + 10*30 = 200 + 300 = 500.00
        Assert.Equal(500.00m, valorTotal);
    }

    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE01_S1_DebeConsumirSoloDeCapaMasAntiguaYCalcularCostosYSaldos()
    {
        // Arrange — Escenario E-01 oficial tras E1 (10 @ 20.00) y E2 (10 @ 30.00)
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = new DateTime(2026, 10, 1),
            CantidadInicial = 10.0000m,
            CantidadDisponible = 10.0000m,
            CostoUnitario = 20.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = new DateTime(2026, 10, 2),
            CantidadInicial = 10.0000m,
            CantidadDisponible = 10.0000m,
            CostoUnitario = 30.0000m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };
        decimal cantidadSalida = 5.0000m;

        // Act — S1: salida de 5 unidades (cabe en capa 1 de 10 @ 20.00)
        var resultado = CalculadoraPeps.CalcularSalida(capas, cantidadSalida);

        // Assert — Oráculo E-01: Costo S1 = 100.00, Saldo = 400.00, Existencia = 15.0000
        Assert.Equal(100.00m, resultado.CostoTotalSalida);
        Assert.Equal(400.00m, resultado.SaldoValorRestante);
        Assert.Equal(15.0000m, resultado.ExistenciaRestante);

        // Capa 1 consumida parcialmente (quedan 5)
        Assert.Equal(5.0000m, capa1.CantidadDisponible);

        // Capa 2 intacta (quedan 10)
        Assert.Equal(10.0000m, capa2.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-08")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_CuandoSalidaAgotaExactamenteCapaMasAntigua_ConsumeTodaLaCapaSinTocarLaSiguiente()
    {
        // Arrange — Capa 1 tiene 5 disponibles, Capa 2 tiene 10 disponibles
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = new DateTime(2026, 10, 1),
            CantidadInicial = 10.0000m,
            CantidadDisponible = 5.0000m,
            CostoUnitario = 20.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = new DateTime(2026, 10, 2),
            CantidadInicial = 10.0000m,
            CantidadDisponible = 10.0000m,
            CostoUnitario = 30.0000m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };
        decimal cantidadSalida = 5.0000m;

        // Act — Consume exactamente los 5 de la capa 1
        var resultado = CalculadoraPeps.CalcularSalida(capas, cantidadSalida);

        // Assert — Costo S = 5 * 20.00 = 100.00, Saldo restante = 10 * 30.00 = 300.00
        Assert.Equal(100.00m, resultado.CostoTotalSalida);
        Assert.Equal(300.00m, resultado.SaldoValorRestante);
        Assert.Equal(10.0000m, resultado.ExistenciaRestante);

        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(10.0000m, capa2.CantidadDisponible);
    }
}
