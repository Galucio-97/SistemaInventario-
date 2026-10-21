using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SI.Web.Data.Entities;

namespace SI.Web.Data.Configurations;

public class CapasPepsConfiguration : IEntityTypeConfiguration<CapasPeps>
{
    public void Configure(EntityTypeBuilder<CapasPeps> builder)
    {
        builder.ToTable("CapasPeps", t =>
        {
            t.HasCheckConstraint("CK_CapasPeps_CantidadDisponible", "[CantidadDisponible] >= 0");
        });
        
        builder.HasKey(cp => cp.Id);

        // Relación obligatoria con Producto (No permitir borrado en cascada - SI-007)
        builder.HasOne(cp => cp.Producto)
            .WithMany()
            .HasForeignKey(cp => cp.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación obligatoria con Producto (No permitir borrado en cascada - SI-007)
        builder.HasOne(cp => cp.Movimiento)
            .WithMany()
            .HasForeignKey(cp => cp.MovimientoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(cp => cp.FechaEntrada)
            .HasColumnType("datetime2")
            .IsRequired();

        // Precisiones numéricas estrictas (Política Sección 5.2 y RB-08)
        builder.Property(cp => cp.CantidadInicial)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(cp => cp.CantidadDisponible)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(cp => cp.CostoUnitario)
            .HasPrecision(18, 4)
            .IsRequired();

        // Índice para recuperar capas en orden estricto de antigüedad FIFO
        builder.HasIndex(cp => new { cp.ProductoId, cp.FechaEntrada, cp.Id })
            .HasDatabaseName("IX_CapasPeps_Producto_FechaEntrada_Id");
    }
}