using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    // TODO: Unik (StudentId, YearWeek). YearWeek max 10. Text max 5000.
    // Den här klassen ska konfigurera entiteten WeeklySummary med hjälp av Fluent API.
    // Den ska innehålla all nödvändig konfiguration för att mappa WeeklySummary-entiteten till databastabellen.
    // T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.

    public class WeeklySummaryConfiguration : IEntityTypeConfiguration<WeeklySummaryEntity>
    {
        public void Configure(EntityTypeBuilder<WeeklySummaryEntity> builder)
        {
            builder.ToTable("WeeklySummaries");

            builder.HasKey(ws => ws.Id);
            
            builder.Property(ws => ws.Id)
                .IsRequired();

            builder.Property(ws => ws.StudentId)
                .IsRequired();

            builder.Property(ws => ws.YearWeek)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(ws => ws.Text)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(ws => ws.CreatedAtUtc)
                .IsRequired();

            builder.Property(ws => ws.UpdatedAtUtc)
                .IsRequired();

            builder.HasIndex(ws => new { ws.StudentId, ws.YearWeek })
                .IsUnique();
        }
        
    }
}