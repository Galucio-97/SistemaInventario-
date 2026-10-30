using System.Text.Json;
using SI.Tests.Fixtures;

namespace SI.Tests.RegressionTests;

public class Def002PortabilidadConexionTests
{
    [Fact]
    [Trait("DEF", "DEF-002")]
    [Trait("Nivel", "Regresion")]
    public void AppSettingsDevelopment_DebeMantenerConfiguracionPortableDocker()
    {
        // Arrange: buscar appsettings.Development.json en el proyecto SI.Web
        var rutaBase = AppContext.BaseDirectory;
        var rutaAppSettings = Path.GetFullPath(Path.Combine(rutaBase, "..", "..", "..", "..", "SI.Web", "appsettings.Development.json"));

        Assert.True(File.Exists(rutaAppSettings), $"No se encontró appsettings.Development.json en la ruta: {rutaAppSettings}");

        var contenidoJson = File.ReadAllText(rutaAppSettings);
        using var jsonDoc = JsonDocument.Parse(contenidoJson);

        var connectionStrings = jsonDoc.RootElement.GetProperty("ConnectionStrings");
        var defaultConn = connectionStrings.GetProperty("DefaultConnection").GetString();
        var testConn = connectionStrings.GetProperty("TestConnection").GetString();

        // Assert: No debe estar acoplado a SQLEXPRESS local ni forzar Integrated Security
        Assert.NotNull(defaultConn);
        Assert.DoesNotContain("SQLEXPRESS", defaultConn, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("localhost,1433", defaultConn, StringComparison.OrdinalIgnoreCase);

        Assert.NotNull(testConn);
        Assert.DoesNotContain("SQLEXPRESS", testConn, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("localhost,1433", testConn, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("DEF", "DEF-002")]
    [Trait("Nivel", "Regresion")]
    public void TestDbContextFixture_DebeTenerFallbackNeutroSinAcoplamientoASqlExpress()
    {
        // Arrange & Act
        var connectionString = TestDbContextFixture.ConnectionString;

        // Assert: Si no hay variable de entorno, el fallback debe ser el contenedor Docker neutro (puerto 1433)
        // y no forzar SQLEXPRESS por detección de sistema operativo.
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__TestConnection")))
        {
            Assert.DoesNotContain("SQLEXPRESS", connectionString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("1433", connectionString);
        }
    }
}
