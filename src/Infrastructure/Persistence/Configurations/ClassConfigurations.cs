using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    // TODO: Unik (SchoolName, Year, ClassName). Maxlängder.
    // Den här klassen ska konfigurera entiteten Class med hjälp av Fluent API.
    // Den ska innehålla all nödvändig konfiguration för att mappa Class-entiteten till databastabellen.
    // T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.
    public class ClassConfigurations : IEntityTypeConfiguration<ClassEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ClassEntity> builder)
        {
            builder.ToTable("Classes");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.SchoolName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Year)
                .IsRequired();

            builder.Property(c => c.ClassName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => new { c.SchoolName, c.Year, c.ClassName })
                .IsUnique();

            // Metadata
            builder.Property(c => c.CreatedAtUtc)
                .IsRequired();

            builder.Property(c => c.UpdatedAtUtc)
                .IsRequired();
        }
        
    }
}