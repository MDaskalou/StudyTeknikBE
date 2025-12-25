using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudySessionConfiguration : IEntityTypeConfiguration<StudySessionsEntity>
    {
        public void Configure(EntityTypeBuilder<StudySessionsEntity> builder)
        {
            builder.ToTable("StudySessions");
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CourseId).IsRequired();
            builder.Property(x => x.SessionGoal).IsRequired().HasMaxLength(500);
            builder.Property(x => x.StartDateUtc).IsRequired();
            builder.Property(x => x.EndDateUtc);
            builder.Property(x => x.PlannedMinutes).IsRequired();
            builder.Property(x => x.ActualMinutes).IsRequired();
            builder.Property(x => x.EnergyStart).IsRequired();
            builder.Property(x => x.EnergyEnd).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.UpdatedAtUtc).IsRequired();

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Course)
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Steps)
                .WithOne(s => s.StudySession)
                .HasForeignKey(s => s.StudySessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CourseId);
            builder.HasIndex(x => x.Status);
        }
    }
}

