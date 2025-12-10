using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudyGoalsConfiguration : IEntityTypeConfiguration<StudyGoalsEntity>
    {
        public void Configure(EntityTypeBuilder<StudyGoalsEntity> builder)
        {
            builder.HasKey(sg => sg.Id);
            
            builder.Property(sg => sg.GoalDescription)
                .IsRequired()
                .HasMaxLength(500);
            
            // FIX: Ändra till NoAction för att undvika cykel med User -> Profile -> Course -> Goal
            builder.HasOne(sg => sg.User)
                .WithMany()
                .HasForeignKey(sg => sg.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction); 
            
            // FIX: Ändra till Cascade. Om kursen raderas, ska målen för kursen försvinna.
            builder.HasOne(sg => sg.Course)
                .WithMany() // (Lägg gärna till navigation property i CourseEntity senare: public ICollection<StudyGoal> StudyGoals { get; set; })
                .HasForeignKey(sg => sg.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}