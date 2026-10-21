using SI.Web.Common;

namespace SI.Tests.UnitTests.Validadores;

public class ValidadorProductoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoCodigoEsVacio_DebeRechazarConMensaje(string? codigoInvalido)
    {
        var resultado = ValidadorProducto.Validar(codigoInvalido, "Tornillo 1/4", "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeCodigoObligatorio, resultado.MensajeError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoNombreEsVacio_DebeRechazarConMensaje(string? nombreInvalido)
    {
        var resultado = ValidadorProducto.Validar("P-001", nombreInvalido, "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeNombreObligatorio, resultado.MensajeError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(string? unidadInvalida)
    {
        var resultado = ValidadorProducto.Validar("P-001", "Tornillo 1/4", unidadInvalida);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeUnidadObligatoria, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoCodigoExcede50Caracteres_DebeRechazarConMensaje()
    {
        var codigoLargo = new string('C', 51);
        var resultado = ValidadorProducto.Validar(codigoLargo, "Tornillo 1/4", "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeCodigoLargo, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoNombreExcede150Caracteres_DebeRechazarConMensaje()
    {
        var nombreLargo = new string('N', 151);
        var resultado = ValidadorProducto.Validar("P-001", nombreLargo, "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeNombreLargo, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoUnidadMedidaExcede30Caracteres_DebeRechazarConMensaje()
    {
        var unidadLarga = new string('U', 31);
        var resultado = ValidadorProducto.Validar("P-001", "Tornillo 1/4", unidadLarga);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeUnidadLarga, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-02")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoDescripcionExcede500Caracteres_DebeRechazarConMensaje()
    {
        var descripcionLarga = new string('D', 501);
        var resultado = ValidadorProducto.Validar("P-001", "Tornillo 1/4", "pieza", descripcionLarga);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeDescripcionLarga, resultado.MensajeError);
    }

    [Fact]
    [Trait("CP", "CP-01")]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Unitaria")]
    public void Validar_CuandoDatosSonValidos_DebeAceptar()
    {
        var resultado = ValidadorProducto.Validar("P-001", "Tornillo 1/4", "pieza", "Acero inoxidable");

        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoDatosSonValidos_DebeAceptar()
    {
        var resultado = ValidadorProducto.ValidarModificacion("Tornillo 1/2", "caja", "Nueva descripcion");

        Assert.True(resultado.EsValido);
        Assert.Null(resultado.MensajeError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoNombreEsVacio_DebeRechazarConMensaje(string? nombreInvalido)
    {
        var resultado = ValidadorProducto.ValidarModificacion(nombreInvalido, "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeNombreObligatorio, resultado.MensajeError);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoNombreExcede150Caracteres_DebeRechazarConMensaje()
    {
        var nombreLargo = new string('N', 151);
        var resultado = ValidadorProducto.ValidarModificacion(nombreLargo, "pieza");

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeNombreLargo, resultado.MensajeError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoUnidadMedidaEsVacia_DebeRechazarConMensaje(string? unidadInvalida)
    {
        var resultado = ValidadorProducto.ValidarModificacion("Tornillo 1/4", unidadInvalida);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeUnidadObligatoria, resultado.MensajeError);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoUnidadMedidaExcede30Caracteres_DebeRechazarConMensaje()
    {
        var unidadLarga = new string('U', 31);
        var resultado = ValidadorProducto.ValidarModificacion("Tornillo 1/4", unidadLarga);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeUnidadLarga, resultado.MensajeError);
    }

    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("RNF", "RNF-03")]
    [Trait("Nivel", "Unitaria")]
    public void ValidarModificacion_CuandoDescripcionExcede500Caracteres_DebeRechazarConMensaje()
    {
        var descripcionLarga = new string('D', 501);
        var resultado = ValidadorProducto.ValidarModificacion("Tornillo 1/4", "pieza", descripcionLarga);

        Assert.False(resultado.EsValido);
        Assert.Equal(ValidadorProducto.MensajeDescripcionLarga, resultado.MensajeError);
    }
}
