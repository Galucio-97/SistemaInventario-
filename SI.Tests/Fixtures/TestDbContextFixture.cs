using Microsoft.EntityFrameworkCore;
using SI.Web.Data;

namespace SI.Tests.Fixtures;

public sealed class TestDbContextFixture : IDisposable
{
    private static readonly object Lock = new();
    private static bool _databaseMigrated;

    // Cadena de conexión configurable mediante variable de entorno (utilizada en CI / local)
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__TestConnection")
        ?? "Server=localhost,1433;Database=SistemaInventarioDb_Tests;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true";

    // Contexto preconfigurado para inspección de Schema / Metamodelo en memoria
    public InventarioDbContext Context { get; }

    public TestDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        
        Context = new InventarioDbContext(options);
    }

    /// <summary>
    /// Garantiza que la BD de pruebas dedicada (SistemaInventarioDb_Tests) se cree y aplique las migraciones automáticamente.
    /// </summary>
    public static void EnsureDatabaseMigrated()
    {
        if (_databaseMigrated) return;

        lock (Lock)
        {
            if (!_databaseMigrated)
            {
                var options = new DbContextOptionsBuilder<InventarioDbContext>()
                    .UseSqlServer(ConnectionString)
                    .Options;

                using var context = new InventarioDbContext(options);
                context.Database.Migrate();
                _databaseMigrated = true;
            }
        }
    }

    /// <summary>
    /// Factory para la creación de contextos independientes que conectan a la BD real de pruebas.
    /// </summary>
    public InventarioDbContext CreateDbContext()
    {
        EnsureDatabaseMigrated();

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        
        return new InventarioDbContext(options);
    }

    /// <summary>
    /// Limpia las tablas de la base de datos de pruebas en orden inverso de claves foráneas
    /// para asegurar que cada prueba inicie en un estado conocido sin datos residuales.
    /// </summary>
    public void ResetDatabase()
    {
        using var context = CreateDbContext();
        context.Database.ExecuteSqlRaw(@"
            DELETE FROM [CapasPeps];
            DELETE FROM [Movimientos];
            DELETE FROM [Productos];
        ");
    }

    /// <summary>
    /// Versión asíncrona de limpieza para pruebas async.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.ExecuteSqlRawAsync(@"
            DELETE FROM [CapasPeps];
            DELETE FROM [Movimientos];
            DELETE FROM [Productos];
        ");
    }

    public void Dispose() => Context.Dispose();
}