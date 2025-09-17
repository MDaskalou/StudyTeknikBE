using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations
{
    // TODO: Unik e-post. Maxlängder. Indexering.
    // Den här klassen ska konfigurera entiteten User med hjälp av Fluent API.
    // Den ska innehålla all nödvändig konfiguration för att mappa User-entiteten till databastabellen.
    // T.ex. Tabellnamn, primärnyckel, egenskaper, relationer etc.
    
    //Vi använder Configurations för att hålla vår konfigurationslogik separat från vår DbContext-klass.
    // Detta gör koden mer organiserad och lättare att underhålla.
    // Det gör det också enklare att återanvända konfigurationer om vi har flera DbContext-klasser i applikationen.
    
    //Configurations används tillsammans med OnModelCreating-metoden i DbContext-klassen.
    // I OnModelCreating kan vi anropa ApplyConfiguration-metoden för att applicera konfigurationen från en separat klass.
    // Detta gör det möjligt att hålla vår DbContext-klass ren och fokuserad på att hantera databaskontexten.
    
    namespace Infrastructure.Persistence.Configurations
    {
        public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
        {
            public void Configure(EntityTypeBuilder<UserEntity> builder)
            {
                builder.ToTable("Users", "dbo");

                builder.HasKey(u => u.Id);
                builder.Property(u => u.Id).ValueGeneratedNever(); // behåll om du sätter Guid i kod

                builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
                builder.HasIndex(u => u.Email).IsUnique();

                builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                builder.Property(u => u.LastName). IsRequired().HasMaxLength(100);
                builder.Property(u => u.Role).     IsRequired();

                builder.Property(u => u.CreatedAtUtc).IsRequired();
                builder.Property(u => u.UpdatedAtUtc).IsRequired();

                // Viktigt: PasswordHash är object i entiteten → ignorera tills du bestämt typ
                builder.Ignore(u => u.PasswordHash);
                // ev. fler fält att ignorera om de är object/komplexa
            }
        }
    }
}