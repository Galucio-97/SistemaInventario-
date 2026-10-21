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

    [Fact]
    [Trait("CP", "CP-15")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE02_Multicapa_DebeConsumirEnOrdenYCalcularCostosYSaldos()
    {
        // Arrange — E-02: Capa 1 (3 @ 10.00), Capa 2 (4 @ 11.00)
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = new DateTime(2026, 10, 1),
            CantidadInicial = 3.0000m,
            CantidadDisponible = 3.0000m,
            CostoUnitario = 10.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = new DateTime(2026, 10, 2),
            CantidadInicial = 4.0000m,
            CantidadDisponible = 4.0000m,
            CostoUnitario = 11.0000m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };

        // Act 1 — S1: Salida de 5 unidades (3 de capa 1 + 2 de capa 2)
        var resultadoS1 = CalculadoraPeps.CalcularSalida(capas, 5.0000m);

        // Assert S1 — Oráculo E-02: Costo S1 = 52.00, Saldo = 22.00, Existencia = 2.0000
        Assert.Equal(52.00m, resultadoS1.CostoTotalSalida);
        Assert.Equal(22.00m, resultadoS1.SaldoValorRestante);
        Assert.Equal(2.0000m, resultadoS1.ExistenciaRestante);
        Assert.Equal(10.4000m, resultadoS1.CostoUnitarioEquivalente); // 52.00 / 5 = 10.4000
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(2.0000m, capa2.CantidadDisponible);

        // Act 2 — S2: Salida de 2 unidades restantes (agota capa 2)
        var resultadoS2 = CalculadoraPeps.CalcularSalida(capas, 2.0000m);

        // Assert S2 — Oráculo E-02: Costo S2 = 22.00, Saldo = 0.00, Existencia = 0.0000
        Assert.Equal(22.00m, resultadoS2.CostoTotalSalida);
        Assert.Equal(0.00m, resultadoS2.SaldoValorRestante);
        Assert.Equal(0.0000m, resultadoS2.ExistenciaRestante);
        Assert.Equal(11.0000m, resultadoS2.CostoUnitarioEquivalente); // 22.00 / 2 = 11.0000
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(0.0000m, capa2.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-17")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE03_ConDecimalesYAgotamiento_DebeConsumirCapasYDejarSaldoCero()
    {
        // Arrange — E-03: Capa 1 (3 @ 10.0000), Capa 2 (3 @ 10.0100)
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = new DateTime(2026, 10, 1),
            CantidadInicial = 3.0000m,
            CantidadDisponible = 3.0000m,
            CostoUnitario = 10.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = new DateTime(2026, 10, 2),
            CantidadInicial = 3.0000m,
            CantidadDisponible = 3.0000m,
            CostoUnitario = 10.0100m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };

        // Act 1 — S1: Salida de 1 unidad de capa 1
        var resultadoS1 = CalculadoraPeps.CalcularSalida(capas, 1.0000m);

        // Assert S1 — Costo = 10.00, Saldo = 50.03, Existencia = 5.0000
        Assert.Equal(10.00m, resultadoS1.CostoTotalSalida);
        Assert.Equal(50.03m, resultadoS1.SaldoValorRestante);
        Assert.Equal(5.0000m, resultadoS1.ExistenciaRestante);
        Assert.Equal(10.0000m, resultadoS1.CostoUnitarioEquivalente);
        Assert.Equal(2.0000m, capa1.CantidadDisponible);
        Assert.Equal(3.0000m, capa2.CantidadDisponible);

        // Act 2 — S2: Salida de 5 unidades (2 @ 10.00 + 3 @ 10.01 = 20.00 + 30.03 = 50.03)
        var resultadoS2 = CalculadoraPeps.CalcularSalida(capas, 5.0000m);

        // Assert S2 — Oráculo E-03: Costo = 50.03, Saldo = 0.00, Existencia = 0.0000
        Assert.Equal(50.03m, resultadoS2.CostoTotalSalida);
        Assert.Equal(0.00m, resultadoS2.SaldoValorRestante);
        Assert.Equal(0.0000m, resultadoS2.ExistenciaRestante);
        Assert.Equal(10.0060m, resultadoS2.CostoUnitarioEquivalente); // 50.03 / 5 = 10.0060
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(0.0000m, capa2.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-16")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_SalidaIgualATodaLaExistencia_DebeAgotarTodasLasCapasYDejarSaldoCero()
    {
        // Arrange — Estado remanente de E-01 (15 unidades en total: 5 @ 20.00 + 10 @ 30.00)
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

        // Act — Salida de exactamente las 15 unidades disponibles
        var resultado = CalculadoraPeps.CalcularSalida(capas, 15.0000m);

        // Assert — Costo = 5*20 + 10*30 = 400.00, Saldo = 0.00, Existencia = 0.0000
        Assert.Equal(400.00m, resultado.CostoTotalSalida);
        Assert.Equal(0.00m, resultado.SaldoValorRestante);
        Assert.Equal(0.0000m, resultado.ExistenciaRestante);
        Assert.Equal(26.6667m, resultado.CostoUnitarioEquivalente); // 400 / 15 = 26.66666... -> 26.6667
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(0.0000m, capa2.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-15")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_CuandoCapaEstaAgotada_DebeOmitirlaYConsumirDeLaSiguienteDisponible()
    {
        // Arrange — Capa 1 agotada (0 disponibles), Capa 2 disponible (5 @ 15.00)
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = new DateTime(2026, 10, 1),
            CantidadInicial = 10.0000m,
            CantidadDisponible = 0.0000m,
            CostoUnitario = 10.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = new DateTime(2026, 10, 2),
            CantidadInicial = 5.0000m,
            CantidadDisponible = 5.0000m,
            CostoUnitario = 15.0000m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };

        // Act — Salida de 2 unidades
        var resultado = CalculadoraPeps.CalcularSalida(capas, 2.0000m);

        // Assert
        Assert.Equal(30.00m, resultado.CostoTotalSalida); // 2 * 15.00
        Assert.Equal(45.00m, resultado.SaldoValorRestante); // 3 * 15.00
        Assert.Equal(3.0000m, resultado.ExistenciaRestante);
        Assert.Equal(15.0000m, resultado.CostoUnitarioEquivalente);
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(3.0000m, capa2.CantidadDisponible);
    }

    [Fact]
    [Trait("CP", "CP-15")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_ConCapasDeMismaFecha_DebeRespetarOrdenPorId()
    {
        // Arrange — Dos capas el mismo día: Id 1 (5 @ 10.00) e Id 2 (5 @ 20.00)
        var fechaMisma = new DateTime(2026, 10, 1);
        var capa1 = new CapasPeps
        {
            Id = 1,
            FechaEntrada = fechaMisma,
            CantidadInicial = 5.0000m,
            CantidadDisponible = 5.0000m,
            CostoUnitario = 10.0000m
        };
        var capa2 = new CapasPeps
        {
            Id = 2,
            FechaEntrada = fechaMisma,
            CantidadInicial = 5.0000m,
            CantidadDisponible = 5.0000m,
            CostoUnitario = 20.0000m
        };
        var capas = new List<CapasPeps> { capa1, capa2 };

        // Act — Salida de 6 unidades (5 de Id 1 + 1 de Id 2)
        var resultado = CalculadoraPeps.CalcularSalida(capas, 6.0000m);

        // Assert — 5 * 10 + 1 * 20 = 70.00. Saldo restante = 4 * 20 = 80.00
        Assert.Equal(70.00m, resultado.CostoTotalSalida);
        Assert.Equal(80.00m, resultado.SaldoValorRestante);
        Assert.Equal(4.0000m, resultado.ExistenciaRestante);
        Assert.Equal(11.6667m, resultado.CostoUnitarioEquivalente); // 70 / 6 = 11.66666... -> 11.6667
        Assert.Equal(0.0000m, capa1.CantidadDisponible);
        Assert.Equal(4.0000m, capa2.CantidadDisponible);
    }
}
