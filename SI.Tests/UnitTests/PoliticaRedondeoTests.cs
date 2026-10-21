using SI.Web.Services;

namespace SI.Tests.UnitTests;

public class PoliticaRedondeoTests
{
    // Valores de la tabla 5.1 del documento 02 (2 decimales, redondeo bancario)
    public static TheoryData<decimal, decimal> CasosImporte => new()
    {
        { 10.005m, 10.00m }, // anterior (0) es par  -> se conserva
        { 10.015m, 10.02m }, // anterior (1) es impar -> sube a par
        { 10.0051m, 10.01m }, // hay dígitos después del 5 -> sube
        { 50.025m, 50.02m }, // anterior (2) es par  -> se conserva
    };

    // Casos a 4 decimales
    public static TheoryData<decimal, decimal> CasosCostoUnitario => new()
    {
        { 10.00005m, 10.0000m },
        { 10.00015m, 10.0002m },
    };

    [Theory]
    [MemberData(nameof(CasosImporte))]
    [Trait("CP", "CP-17")]
    [Trait("RB", "RB-09")]
    [Trait("RF", "RF-04")]

[Trait("Nivel", "Unitaria")]
    public void Importe_RedondeoBancario_A2Decimales(decimal entrada, decimal esperado)
    {
        var resultado = PoliticaRedondeo.Importe(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [MemberData(nameof(CasosCostoUnitario))]
    [Trait("CP", "CP-17")]
    [Trait("RB", "RB-09")]
    [Trait("Nivel", "Unitaria")]
    [Trait("RF", "RF-04")]
    public void CostoUnitario_RedondeoBancario_A4Decimales(decimal entrada, decimal esperado)
    {
        var resultado = PoliticaRedondeo.CostoUnitario(entrada);

        Assert.Equal(esperado, resultado);
    }
}