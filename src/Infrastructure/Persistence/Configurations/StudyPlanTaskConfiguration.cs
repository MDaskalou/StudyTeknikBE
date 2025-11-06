using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudyPlanTaskConfiguration : IEntityTypeConfiguration<StudyPlanTask>
    {
        public void Configure(EntityTypeBuilder<StudyPlanTask> builder)
        {
            builder.HasKey(sg => sg.Id);
            
            builder.Property(sg => sg.TaskDescription)
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