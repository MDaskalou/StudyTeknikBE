using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<CourseEntity>
    {
        public void Configure(EntityTypeBuilder<CourseEntity> builder)
        {
            builder.ToTable("Courses"); // Namnet på tabellen i SQL

            builder.HasKey(x => x.Id);

            // Egenskaper
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200); // Bra praxis att sätta gränser

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            // ENUM HANTERING
            // EF Core sparar Enums som int (1, 2, 3) automatiskt.
            // Om du vill spara som sträng ("Hard") använder du .HasConversion<string>()
            builder.Property(x => x.Difficulty)
                .IsRequired(); 

            // Relationer
            // En kurs MÅSTE tillhöra en profil
            builder.HasOne(x => x.StudentProfile)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade); // Tar man bort profilen, försvinner kurserna
        }
    }
}