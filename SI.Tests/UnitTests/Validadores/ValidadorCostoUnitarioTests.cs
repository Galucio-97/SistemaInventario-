using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorCostoUnitarioTests
{
    [Theory]
    [InlineData(-0.01)]
    [InlineData(0)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(decimal costoInvalido)
    {
        // Act
        var resultado = ValidadorCostoUnitario.Validar(costoInvalido);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.Equal("El costo unitario debe ser mayor que cero.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.00001)]
    [InlineData(1.00001)]
    [InlineData(10.12345)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(decimal costoInvalido)
    {
        // Act
        var resultado = ValidadorCostoUnitario.Validar(costoInvalido);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.Equal("El costo unitario no puede tener más de 4 decimales.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(0.01)]
    [InlineData(20.00)]
    [InlineData(100.1234)]
    [Trait("CP", "CP-06")]
    [Trait("RF", "RB-02")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCostoUnitario_CuandoEsValido_DebeAceptar(decimal costoValido)
    {
        // Act
        var resultado = ValidadorCostoUnitario.Validar(costoValido);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }
}
