using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    //Todo: Unik (StudentId, CourseId). DateOnly → date.
    //Den här klassen ska konfigurera entiteten Enrollment med hjälp av Fluent API.
    //Den ska innehålla all nödvändig konfiguration för att mappa Enrollment
        
    public class EnrollmentConfiguration : IEntityTypeConfiguration<EnrollmentEntity>
    {
        public void Configure(EntityTypeBuilder<EnrollmentEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();
            
            builder.HasIndex(e => new { e.StudentId, e.ClassId }).IsUnique();
            
            builder.Property(e => e.CreatedAtUtc).IsRequired();
            builder.Property(e => e.UpdatedAtUtc).IsRequired();
            
            builder.Property(e => e.StudentId).IsRequired();
            builder.Property(e => e.ClassId).IsRequired();
            
            // No navigation properties in MVP
            // builder.HasOne(e => e.Student)
            //     .WithMany() // No navigation property in UserEntity
            //     .HasForeignKey(e => e.StudentId)
            //     .OnDelete(DeleteBehavior.Cascade);
            //
            // builder.HasOne(e => e.Class)
            //     .WithMany(c => c.Enrollments) // Navigation property in ClassEntity
            //     .HasForeignKey(e => e.ClassId)
            //     .OnDelete(DeleteBehavior.Cascade);
        }
        
    }
}