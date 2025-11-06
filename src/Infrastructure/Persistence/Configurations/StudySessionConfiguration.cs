using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudySessionConfiguration : IEntityTypeConfiguration<StudySession>
    {
        public void Configure(EntityTypeBuilder<StudySession> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TaskDescription).HasMaxLength(500);
            builder.Property(s => s.WorkFeedback).HasMaxLength(50);
            builder.Property(s => s.BreakFeedback).HasMaxLength(50);

            // Relation: Ett 'User' kan ha många 'Sessions'
            builder.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Ett 'Subject' kan ha många 'Sessions'
            builder.HasOne<SubjectEntity>()
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}