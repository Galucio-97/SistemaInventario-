using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SI.Web.Data.Entities;

namespace SI.Web.Data.Configurations;

public class MovimientoConfiguration : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> builder)
    {
        builder.ToTable("Movimientos");
        
        builder.HasKey(m => m.Id);
        
        // Relación obligatoria con Producto (No permitir borrado en cascada - SI-007)
        builder.HasOne(m => m.Producto)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(m => m.Fecha)
            .HasColumnType("datetime2")
            .IsRequired();
        
        builder.Property(m => m.Tipo)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();
        
        // Precisiones numéricas estrictas (Política Sección 5.2 y RB-08)
        builder.Property(m => m.Cantidad)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(m => m.ExistenciaResultante)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(m => m.CostoUnitarioEntrada)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(m => m.CostoUnitarioPeps)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(m => m.CostoUnitarioPromedio)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(m => m.CostoTotalPeps)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(m => m.SaldoValorPeps)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(m => m.CostoTotalPromedio)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(m => m.SaldoValorPromedio)
            .HasPrecision(18, 2)
            .IsRequired();
        
        // Indice de rendimiento de Kardex e histórico cronológico (RF-05)
        builder.HasIndex(m => new { m.ProductoId, m.Fecha, m.Id })
            .HasDatabaseName("IX_Movimientos_Producto_Fecha_Id");
    }
}