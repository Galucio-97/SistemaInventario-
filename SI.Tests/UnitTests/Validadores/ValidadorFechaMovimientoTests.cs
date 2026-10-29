using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorFechaMovimientoTests
{
    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoFechaEsPosteriorAUltimoMovimiento_DebeAceptar()
    {
        // Arrange
        var fechaNuevo = new DateTime(2026, 10, 15, 10, 0, 0);
        var fechaUltimo = new DateTime(2026, 10, 14, 18, 0, 0);

        // Act
        var resultado = ValidadorFechaMovimiento.Validar(fechaNuevo, fechaUltimo);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoFechaEsIgualAUltimoMovimiento_DebeAceptar()
    {
        // Arrange
        var fechaNuevo = new DateTime(2026, 10, 15, 10, 0, 0);
        var fechaUltimo = new DateTime(2026, 10, 15, 10, 0, 0);

        // Act
        var resultado = ValidadorFechaMovimiento.Validar(fechaNuevo, fechaUltimo);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoFechaEsAnteriorAUltimoMovimiento_DebeRechazarConMensaje()
    {
        // Arrange
        var fechaNuevo = new DateTime(2026, 10, 13, 10, 0, 0);
        var fechaUltimo = new DateTime(2026, 10, 14, 10, 0, 0);

        // Act
        var resultado = ValidadorFechaMovimiento.Validar(fechaNuevo, fechaUltimo);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorFechaMovimiento.MensajeFechaRetroactiva, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-20")]
    [Trait("RB", "RB-11")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoNoExistenMovimientosPrevios_DebeAceptar()
    {
        // Arrange
        var fechaNuevo = new DateTime(2026, 10, 15, 10, 0, 0);
        DateTime? fechaUltimo = null;

        // Act
        var resultado = ValidadorFechaMovimiento.Validar(fechaNuevo, fechaUltimo);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }
}
