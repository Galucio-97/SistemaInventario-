using Xunit.Abstractions;

namespace SI.Tests.UnitTests;

public class UnitTestCase(ITestOutputHelper testOutputHelper)
{
    [Fact]
    [Trait("Nivel", "Boilerplate")]
    public void IsTesting_EnvironmentWorking_ReturnTrue()
    {
        // Arrange - Preparación de la prueba
        testOutputHelper.WriteLine(Environment.CurrentDirectory);
        
        // Act - Acciones que hacen que la prueba tenga valía
        // Assert - Validación de resultados para comprobar que funciona como tal
        Assert.True(true);
    }
}