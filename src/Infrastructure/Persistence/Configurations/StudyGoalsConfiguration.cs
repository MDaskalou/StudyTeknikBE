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
            
            builder.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(sg => sg.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne<SubjectEntity>()
                .WithMany()
                .HasForeignKey(sg => sg.SubjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}