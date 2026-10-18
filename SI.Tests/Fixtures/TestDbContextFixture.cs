using Microsoft.EntityFrameworkCore;
using SI.Web.Data;

namespace SI.Tests.Fixtures;

public sealed class TestDbContextFixture : IDisposable
{
    // Contexto preconfigurado en memoria para inspección de Schema
    public InventarioDbContext Context { get; }

    public TestDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer()
            .Options;
        
        Context = new InventarioDbContext(options);
    }

    // Factory para la creación de contextos independientes al momento de hacer queries a BD de pruebas
    public InventarioDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=SistemaInventarioDb_Tests;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true")
            .Options;
        
        return new InventarioDbContext(options);
    }

    public void Dispose() => Context.Dispose();
}