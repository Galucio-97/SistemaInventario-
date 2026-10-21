using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorCantidadTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RB-01")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCantidad_CuandoEsMenorOIgualACero_DebeRechazarConMensaje(decimal cantidadInvalida)
    {
        // Act
        var resultado = ValidadorCantidad.Validar(cantidadInvalida);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.Equal("La cantidad debe ser mayor que cero.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.00001)]
    [InlineData(1.00001)]
    [InlineData(10.12345)]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RB-01")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCantidad_CuandoTieneMasDeCuatroDecimales_DebeRechazarConMensaje(decimal cantidadInvalida)
    {
        // Act
        var resultado = ValidadorCantidad.Validar(cantidadInvalida);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.Equal("La cantidad no puede tener más de 4 decimales.", resultado.MensajeError);
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(1)]
    [InlineData(10.5)]
    [InlineData(100.1234)]
    [Trait("CP", "CP-05")]
    [Trait("RF", "RB-01")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarCantidad_CuandoEsValida_DebeAceptar(decimal cantidadValida)
    {
        // Act
        var resultado = ValidadorCantidad.Validar(cantidadValida);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }
}