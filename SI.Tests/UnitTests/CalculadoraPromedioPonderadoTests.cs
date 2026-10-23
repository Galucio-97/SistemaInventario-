using SI.Web.Services;

namespace SI.Tests.UnitTests;

public class CalculadoraPromedioPonderadoTests
{
    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularEntrada_EscenarioE01_PrimeraEntrada_CalculaPromedioYSaldoCorrectos()
    {
        // Arrange (E-01 / E1: 10 unidades @ 20.00 sobre existencia inicial 0)
        decimal existenciaActual = 0m;
        decimal saldoValorActual = 0.00m;
        decimal cantidadEntrada = 10m;
        decimal costoUnitarioEntrada = 20.00m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularEntrada(
            existenciaActual,
            saldoValorActual,
            cantidadEntrada,
            costoUnitarioEntrada);

        // Assert
        Assert.Equal(10m, resultado.Existencia);
        Assert.Equal(200.00m, resultado.SaldoValor);
        Assert.Equal(20.0000m, resultado.CostoPromedio);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularEntrada_EscenarioE01_SegundaEntrada_RecalculaPromedioPonderadoYSaldo()
    {
        // Arrange (E-01 / E2: 10 unidades @ 30.00 tras E1 que dejó 10 @ 200.00)
        decimal existenciaActual = 10m;
        decimal saldoValorActual = 200.00m;
        decimal cantidadEntrada = 10m;
        decimal costoUnitarioEntrada = 30.00m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularEntrada(
            existenciaActual,
            saldoValorActual,
            cantidadEntrada,
            costoUnitarioEntrada);

        // Assert: (200.00 + 300.00) / (10 + 10) = 500 / 20 = 25.0000
        Assert.Equal(20m, resultado.Existencia);
        Assert.Equal(500.00m, resultado.SaldoValor);
        Assert.Equal(25.0000m, resultado.CostoPromedio);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularEntrada_EscenarioE02_SegundaEntrada_AplicaRedondeoBancarioACuatroDecimales()
    {
        // Arrange (E-02 / E2: tras E1 con 3 unidades y saldo 30.00, entra 4 @ 11.00)
        decimal existenciaActual = 3m;
        decimal saldoValorActual = 30.00m;
        decimal cantidadEntrada = 4m;
        decimal costoUnitarioEntrada = 11.00m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularEntrada(
            existenciaActual,
            saldoValorActual,
            cantidadEntrada,
            costoUnitarioEntrada);

        // Assert: 74 / 7 = 10.571428... -> 10.5714 (4 decimales redondeo bancario)
        Assert.Equal(7m, resultado.Existencia);
        Assert.Equal(74.00m, resultado.SaldoValor);
        Assert.Equal(10.5714m, resultado.CostoPromedio);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularEntrada_EscenarioE03_SegundaEntrada_AplicaRedondeoBancarioParImpar()
    {
        // Arrange (E-03 / E2: tras E1 con 3 unidades y saldo 30.00, entra 3 @ 10.01)
        decimal existenciaActual = 3m;
        decimal saldoValorActual = 30.00m;
        decimal cantidadEntrada = 3m;
        decimal costoUnitarioEntrada = 10.01m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularEntrada(
            existenciaActual,
            saldoValorActual,
            cantidadEntrada,
            costoUnitarioEntrada);

        // Assert: 60.03 / 6 = 10.0050 exacto -> redondeo bancario a par -> 10.0050
        Assert.Equal(6m, resultado.Existencia);
        Assert.Equal(60.03m, resultado.SaldoValor);
        Assert.Equal(10.0050m, resultado.CostoPromedio);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularEntrada_ConCantidadTotalCero_NoLanzaDivisionPorCero()
    {
        // Arrange: caso borde donde cantidad total resulta cero
        decimal existenciaActual = 0m;
        decimal saldoValorActual = 0.00m;
        decimal cantidadEntrada = 0m;
        decimal costoUnitarioEntrada = 0m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularEntrada(
            existenciaActual,
            saldoValorActual,
            cantidadEntrada,
            costoUnitarioEntrada);

        // Assert: debe manejarlo de forma segura sin DivideByZeroException
        Assert.Equal(0m, resultado.Existencia);
        Assert.Equal(0.00m, resultado.SaldoValor);
        Assert.Equal(0.0000m, resultado.CostoPromedio);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE01_SalidaParcial_MantieneCostoPromedioYDescuentaSaldo()
    {
        // Arrange (E-01 / S1: Sale 5 unidades sobre existencia 20, saldo 500.00, promedio 25.0000)
        decimal existenciaActual = 20m;
        decimal saldoValorActual = 500.00m;
        decimal costoPromedioActual = 25.0000m;
        decimal cantidadSalida = 5m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularSalida(
            existenciaActual,
            saldoValorActual,
            costoPromedioActual,
            cantidadSalida);

        // Assert: 5 * 25.0000 = 125.00 costo salida, saldo remanente 375.00, promedio se mantiene
        Assert.Equal(15m, resultado.Existencia);
        Assert.Equal(375.00m, resultado.SaldoValor);
        Assert.Equal(25.0000m, resultado.CostoPromedio);
        Assert.Equal(125.00m, resultado.CostoTotalSalida);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE02_SalidaS1_CalculaCostoConCuatroDecimalesYRedondeoImporte()
    {
        // Arrange (E-02 / S1: Sale 5 unidades sobre existencia 7, saldo 74.00, promedio 10.5714)
        decimal existenciaActual = 7m;
        decimal saldoValorActual = 74.00m;
        decimal costoPromedioActual = 10.5714m;
        decimal cantidadSalida = 5m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularSalida(
            existenciaActual,
            saldoValorActual,
            costoPromedioActual,
            cantidadSalida);

        // Assert: 5 * 10.5714 = 52.857 -> 52.86; saldo = 74.00 - 52.86 = 21.14
        Assert.Equal(2m, resultado.Existencia);
        Assert.Equal(21.14m, resultado.SaldoValor);
        Assert.Equal(10.5714m, resultado.CostoPromedio);
        Assert.Equal(52.86m, resultado.CostoTotalSalida);
    }

    [Fact]
    [Trait("CP", "CP-09")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("RB", "D-03")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE02_SalidaS2_AgotamientoTotal_AjustaResiduoYDejaSaldoEnCero()
    {
        // Arrange (E-02 / S2: Sale 2 unidades sobre existencia 2, saldo 21.14, promedio 10.5714)
        decimal existenciaActual = 2m;
        decimal saldoValorActual = 21.14m;
        decimal costoPromedioActual = 10.5714m;
        decimal cantidadSalida = 2m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularSalida(
            existenciaActual,
            saldoValorActual,
            costoPromedioActual,
            cantidadSalida);

        // Assert: Agota existencia -> saldo queda 0.00 y costo salida absorbe saldo restante (21.14)
        Assert.Equal(0m, resultado.Existencia);
        Assert.Equal(0.00m, resultado.SaldoValor);
        Assert.Equal(0.0000m, resultado.CostoPromedio);
        Assert.Equal(21.14m, resultado.CostoTotalSalida);
    }

    [Fact]
    [Trait("CP", "CP-17")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-09")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE03_SalidaS1_AplicaRedondeoBancarioParImpar()
    {
        // Arrange (E-03 / S1: Sale 1 unidad sobre existencia 6, saldo 60.03, promedio 10.0050)
        decimal existenciaActual = 6m;
        decimal saldoValorActual = 60.03m;
        decimal costoPromedioActual = 10.0050m;
        decimal cantidadSalida = 1m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularSalida(
            existenciaActual,
            saldoValorActual,
            costoPromedioActual,
            cantidadSalida);

        // Assert: 1 * 10.0050 = 10.005 -> bancario al par 10.00 (half-up daría 10.01)
        Assert.Equal(5m, resultado.Existencia);
        Assert.Equal(50.03m, resultado.SaldoValor);
        Assert.Equal(10.0050m, resultado.CostoPromedio);
        Assert.Equal(10.00m, resultado.CostoTotalSalida);
    }

    [Fact]
    [Trait("CP", "CP-17")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-09")]
    [Trait("RB", "D-03")]
    [Trait("RNF", "RNF-01")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_EscenarioE03_SalidaS2_AgotamientoTotal_AjustaCentavoResiduoD03()
    {
        // Arrange (E-03 / S2: Sale 5 unidades sobre existencia 5, saldo 50.03, promedio 10.0050)
        // 5 * 10.0050 = 50.025 -> bancario daría 50.02 dejando residuo de 0.01.
        // D-03 exige ajustar el costo de salida al saldo restante (50.03) para que el saldo quede en 0.00.
        decimal existenciaActual = 5m;
        decimal saldoValorActual = 50.03m;
        decimal costoPromedioActual = 10.0050m;
        decimal cantidadSalida = 5m;

        // Act
        var resultado = CalculadoraPromedioPonderado.CalcularSalida(
            existenciaActual,
            saldoValorActual,
            costoPromedioActual,
            cantidadSalida);

        // Assert
        Assert.Equal(0m, resultado.Existencia);
        Assert.Equal(0.00m, resultado.SaldoValor);
        Assert.Equal(0.0000m, resultado.CostoPromedio);
        Assert.Equal(50.03m, resultado.CostoTotalSalida);
    }

    [Fact]
    [Trait("CP", "CP-07")]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-03")]
    [Trait("Nivel", "Unitaria")]
    public void CalcularSalida_CuandoSalidaMayorAExistencia_LanzaInvalidOperationException()
    {
        // Arrange: intento de salida de 10 con existencia 5
        decimal existenciaActual = 5m;
        decimal saldoValorActual = 50.00m;
        decimal costoPromedioActual = 10.0000m;
        decimal cantidadSalida = 10m;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            CalculadoraPromedioPonderado.CalcularSalida(
                existenciaActual,
                saldoValorActual,
                costoPromedioActual,
                cantidadSalida));
    }
}
