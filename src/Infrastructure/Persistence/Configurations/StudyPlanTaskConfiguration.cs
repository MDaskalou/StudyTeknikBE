using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudyPlanTaskConfiguration : IEntityTypeConfiguration<StudyPlanTasksEntity>
    {
        public void Configure(EntityTypeBuilder<StudyPlanTasksEntity> builder)
        {
            // Bytte variabelnamn från sg -> t för tydlighet
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Cascade är helt rätt här. Tar man bort målet ska uppgifterna försvinna.
            builder.HasOne<StudyGoalsEntity>()
                .WithMany()
                .HasForeignKey(t => t.StudyGoalId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}