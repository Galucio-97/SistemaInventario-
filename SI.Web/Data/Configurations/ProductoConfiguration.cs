using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SI.Web.Data.Entities;

namespace SI.Web.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos");
        
        builder.HasKey(p => p.Id);
        
        // Clave Única (RF-01, CP-02)
        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_Productos_Codigo");
        
        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(p => p.Descripcion)
            .HasMaxLength(500)
            .IsRequired(false);
        
        builder.Property(p => p.UnidadMedida)
            .IsRequired()
            .HasMaxLength(30);
        
        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Precisiones numéricas estrictas (Corrección DEF-001 y RB-08)
        builder.Property(p => p.ExistenciaActual)
            .HasPrecision(18, 4)
            .HasDefaultValue(0m);
        
        builder.Property(p => p.CostoPromedioActual)
            .HasPrecision(18, 4)
            .HasDefaultValue(0m);
        
        builder.Property(p => p.ValorInventarioPeps)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);
        
        builder.Property(p => p.ValorInventarioPromedio)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);
    }
}