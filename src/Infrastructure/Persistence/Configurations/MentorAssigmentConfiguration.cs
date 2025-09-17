using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MentorAssigmentConfiguration : IEntityTypeConfiguration<MentorAssigmentEntity>
    {
        public void Configure(EntityTypeBuilder<MentorAssigmentEntity> builder)
        {
            builder.ToTable("MentorAssigments");

            builder.HasKey(ma => ma.Id);
            
            builder.Property(ma => ma.Id)
                .IsRequired();

            builder.Property(ma => ma.MentorId)
                .IsRequired();

            builder.Property(ma => ma.StudentId)
                .IsRequired();

            builder.HasIndex(ma => new { ma.MentorId, ma.StudentId })
                .IsUnique();
        }
        
    }
}