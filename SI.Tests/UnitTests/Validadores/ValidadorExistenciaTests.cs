using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorExistenciaTests
{
    [Theory]
    [InlineData(14.9999, 15.0000)] // Frontera inferior válida
    [InlineData(15.0000, 15.0000)] // Salida exacta (CP-16)
    [InlineData(0.0001, 10.0000)]  // Salida mínima válida
    [Trait("CP", "CP-07")]
    [Trait("CP", "CP-16")]
    [Trait("RF", "RB-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoCantidadMenorOIgualAExistencia_DebeAceptar(decimal cantidadSalida, decimal existenciaDisponible)
    {
        // Act
        var resultado = ValidadorExistencia.Validar(cantidadSalida, existenciaDisponible);

        // Assert
        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }

    [Theory]
    [InlineData(15.0001, 15.0000)] // Frontera superior inválida
    [InlineData(20.0000, 15.0000)] // Salida muy superior
    [InlineData(1.0000, 0.0000)]   // Salida sin existencia disponible
    [Trait("CP", "CP-07")]
    [Trait("RF", "RB-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoCantidadMayorAExistencia_DebeRechazarConMensaje(decimal cantidadSalida, decimal existenciaDisponible)
    {
        // Act
        var resultado = ValidadorExistencia.Validar(cantidadSalida, existenciaDisponible);

        // Assert
        Assert.False(resultado.EsValido);
        Assert.NotNull(resultado.MensajeError);
        Assert.Contains("Existencia insuficiente", resultado.MensajeError);
        Assert.Contains(existenciaDisponible.ToString(), resultado.MensajeError);
    }
}
