using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    // TODO: Unik (StudentId, EntryDate). Text max 5000. DateOnly → date.
    // Den här klassen ska konfigurera entiteten DiaryEntry med hjälp av Fluent API.
    // Den ska innehålla all nödvändig konfiguration för att mappa DiaryEntry-entiteten till databastabellen.
    // T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.

    public class DiaryEntryConfiguration : IEntityTypeConfiguration<DiaryEntryEntity>
    {
        public void Configure(EntityTypeBuilder<DiaryEntryEntity> builder)
        {
            builder.ToTable("DiaryEntries", "dbo");
            builder.HasKey(de => de.Id);
            builder.Property(de => de.Id).ValueGeneratedNever();
            builder.Property(de => de.StudentId).IsRequired();
            builder.Property(de => de.EntryDate).IsRequired().HasColumnType("date");
            builder.Property(de => de.Text).IsRequired().HasMaxLength(5000);
            builder.Property(de => de.CreatedAtUtc).IsRequired();
            builder.Property(de => de.UpdatedAtUtc).IsRequired();

            builder.HasIndex(de => new { de.StudentId, de.EntryDate }).IsUnique(); // Unik index på (StudentId, EntryDate)
            
        }

        
    }
}