using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    
    //Todo: Unik (UserId, Action, Timestamp). Text max 2000. DateTime → datetime2.
    //Den här klassen ska konfigurera entiteten AuditLog med hjälp av Fluent API.
    //Den ska innehålla all nödvändig konfiguration för att mappa AuditLog-entiteten till databastabellen.
    //T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntity>
    {
        public void Configure(EntityTypeBuilder<AuditLogEntity> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.Id);
            
            builder.Property(al => al.Id)
                .IsRequired();

            builder.Property(al => al.UserId)
                .IsRequired();

           
        }
        
    }
}