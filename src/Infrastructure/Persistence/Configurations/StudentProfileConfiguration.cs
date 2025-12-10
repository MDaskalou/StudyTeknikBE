using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfileEntity>
    {
        public void Configure(EntityTypeBuilder<StudentProfileEntity> builder)
        {
            builder.ToTable("StudentProfiles");
            builder.HasKey(x => x.Id);

            // Relation: En User har en StudentProfile
            builder.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<StudentProfileEntity>(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade); // Ändrat till Cascade (User borta = Profil borta)

            // Relation: En StudentProfile har många Courses
            builder.HasMany(x => x.Courses)
                .WithOne(x => x.StudentProfile)
                .HasForeignKey(x => x.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade); // Helt rätt.
        }
    }
}