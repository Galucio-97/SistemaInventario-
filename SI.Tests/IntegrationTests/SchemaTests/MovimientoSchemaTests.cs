using Microsoft.EntityFrameworkCore;
using SI.Tests.Fixtures;
using SI.Web.Data;
using SI.Web.Data.Entities;

namespace SI.Tests.IntegrationTests.SchemaTests;

public class MovimientoSchemaTests(TestDbContextFixture fixture) : IClassFixture<TestDbContextFixture>
{
    [Fact]
    [Trait("RF", "RF-02")]
    [Trait("RF", "RF-03")]
    [Trait("Nivel", "Integracion")]
    public void Movimiento_Schema_DebeExistirEnContexto()
    {
        // Arrange
        var context = fixture.Context;
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(Movimiento));
        
        // Assert
        Assert.NotNull(entidad);
    }

    [Fact]
    [Trait("RF", "RF-02")]
    [Trait("RF", "RF-03")]
    [Trait("Nivel", "Integracion")]
    public void Movimiento_Schema_DebeTenerPrecisionDecimalCorrecta()
    {
        // Arrange
        var context = fixture.Context;
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(Movimiento));
        var cantidad = entidad?.FindProperty(nameof(Movimiento.Cantidad));
        var costoUnitario = entidad?.FindProperty(nameof(Movimiento.CostoUnitarioEntrada));
        var existencia = entidad?.FindProperty(nameof(Movimiento.ExistenciaResultante));
        
        var costoUnitarioPeps = entidad?.FindProperty(nameof(Movimiento.CostoUnitarioPeps));
        var costoTotalPeps = entidad?.FindProperty(nameof(Movimiento.CostoTotalPeps));
        var saldoValorPeps = entidad?.FindProperty(nameof(Movimiento.SaldoValorPeps));
        
        var costoUnitarioPromedio = entidad?.FindProperty(nameof(Movimiento.CostoUnitarioPromedio));
        var costoTotalPromedio = entidad?.FindProperty(nameof(Movimiento.CostoTotalPromedio));
        var saldoValorPromedio = entidad?.FindProperty(nameof(Movimiento.SaldoValorPromedio));

        // Assert
        Assert.Equal(typeof(decimal), cantidad?.ClrType);
        Assert.Equal(typeof(decimal), existencia?.ClrType);
        
        Assert.Equal(18, cantidad?.GetPrecision());
        Assert.Equal(4, cantidad?.GetScale());
        
        Assert.Equal(18, costoUnitario?.GetPrecision());
        Assert.Equal(4, costoUnitario?.GetScale());
        
        Assert.Equal(18, existencia?.GetPrecision());
        Assert.Equal(4, existencia?.GetScale());
        
        // ====== Validaciones peps
        Assert.Equal(18, costoUnitarioPeps?.GetPrecision());
        Assert.Equal(4, costoUnitarioPeps?.GetScale());
        
        Assert.Equal(18, costoTotalPeps?.GetPrecision());
        Assert.Equal(2, costoTotalPeps?.GetScale());
        
        Assert.Equal(18, saldoValorPeps?.GetPrecision());
        Assert.Equal(2, saldoValorPeps?.GetScale());
        
        // ====== Validaciones costo promedio ponderado
        Assert.Equal(18, costoUnitarioPromedio?.GetPrecision());
        Assert.Equal(4, costoUnitarioPromedio?.GetScale());
        
        Assert.Equal(18, costoTotalPromedio?.GetPrecision());
        Assert.Equal(2, costoTotalPromedio?.GetScale());
        
        Assert.Equal(18, saldoValorPromedio?.GetPrecision());
        Assert.Equal(2, saldoValorPromedio?.GetScale());
    }
}