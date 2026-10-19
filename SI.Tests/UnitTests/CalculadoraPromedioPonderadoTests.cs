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
}
