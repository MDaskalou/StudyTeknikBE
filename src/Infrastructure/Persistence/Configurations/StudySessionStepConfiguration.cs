using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StudySessionStepConfiguration : IEntityTypeConfiguration<StudySessionStepEntity>
    {
        public void Configure(EntityTypeBuilder<StudySessionStepEntity> builder)
        {
            builder.ToTable("StudySessionSteps");
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StudySessionId).IsRequired();
            builder.Property(x => x.OrderIndex).IsRequired();
            builder.Property(x => x.StepType).IsRequired();
            builder.Property(x => x.Description).IsRequired().HasMaxLength(300);
            builder.Property(x => x.DurationMinutes).IsRequired();
            builder.Property(x => x.IsCompleted).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();

            // Relationships
            builder.HasOne(x => x.StudySession)
                .WithMany(s => s.Steps)
                .HasForeignKey(x => x.StudySessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.StudySessionId);
            builder.HasIndex(x => x.OrderIndex);
        }
    }
}

