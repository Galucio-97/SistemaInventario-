using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SI.Tests.Fixtures;
using SI.Web.Data.Entities;

namespace SI.Tests.IntegrationTests.SchemaTests;

public class CapasPepsSchemaTests(TestDbContextFixture fixture) : IClassFixture<TestDbContextFixture>
{
    [Fact]
    [Trait("RF", "RF-01")]
    [Trait("Nivel", "Integracion")]
    public void CapasInventarioPeps_Schema_DebeExistirEnContexto()
    {
        // Arrange
        var context = fixture.Context;
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(CapasPeps));
        
        // Assert
        Assert.NotNull(entidad);
    }
    
    [Fact]
    [Trait("RF", "RF-02")]
    [Trait("RF", "RF-03")]
    [Trait("Nivel", "Integracion")]
    public void CapasInventarioPeps_Schema_DebeTenerPrecisionDecimalCorrecta()
    {
        // Arrange
        var context = fixture.Context;
        
        // Act
        var entidad = context.Model.FindEntityType(typeof(CapasPeps));
        var cantidadInicial = entidad?.FindProperty(nameof(CapasPeps.CantidadInicial));
        var cantidadDisponible= entidad?.FindProperty(nameof(CapasPeps.CantidadInicial)); 
        var costoUnitario = entidad?.FindProperty(nameof(CapasPeps.CostoUnitario));

        // Assert
        Assert.Equal(18, cantidadInicial?.GetPrecision());
        Assert.Equal(4, cantidadInicial?.GetScale());
        
        Assert.Equal(18, cantidadDisponible?.GetPrecision());
        Assert.Equal(4, cantidadDisponible?.GetScale());
        
        Assert.Equal(18, costoUnitario?.GetPrecision());
        Assert.Equal(4, costoUnitario?.GetScale());
    }
    
    [Fact]
    [Trait("RF", "RF-04")]
    [Trait("RB", "RB-07")]
    [Trait("Nivel", "Integracion")]
    public void CapasInventarioPeps_Schema_DebeTenerRelacionesYCheckConstraint()
    {
        // Arrange
        var context = fixture.Context;
        var entidad = context.Model.FindEntityType(typeof(CapasPeps));
        
        // Act
        var fkProducto = entidad?.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Producto));
        
        var fkMovimiento = entidad?.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Movimiento));
        
        // Assert
        Assert.NotNull(fkProducto);
        Assert.Equal(DeleteBehavior.Restrict, fkProducto.DeleteBehavior);
        
        Assert.NotNull(fkMovimiento);
        Assert.Equal(DeleteBehavior.Restrict, fkMovimiento.DeleteBehavior);
        
        // Check Constraint: CantidadDisponible >= 0
        var checkConstraintModel  = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(CapasPeps));
        
        var checkConstraint = checkConstraintModel?.GetCheckConstraints()
            .FirstOrDefault(c => c.Name != null && c.Name.Contains("CantidadDisponible"));
        
        Assert.NotNull(checkConstraint);
    }
}