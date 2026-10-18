using Microsoft.EntityFrameworkCore;
using SI.Web.Data.Entities;

namespace SI.Web.Data;

public class InventarioDbContext(DbContextOptions<InventarioDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventarioDbContext).Assembly);
    }
}