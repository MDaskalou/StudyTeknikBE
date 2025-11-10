using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudyPlanTaskConfiguration : IEntityTypeConfiguration<StudyPlanTasksEntity>
    {
        public void Configure(EntityTypeBuilder<StudyPlanTasksEntity> builder)
        {
            builder.HasKey(sg => sg.Id);
            
            builder.Property(sg => sg.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne<StudyGoalsEntity>()
                .WithMany()
                .HasForeignKey(sg => sg.StudyGoalId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}