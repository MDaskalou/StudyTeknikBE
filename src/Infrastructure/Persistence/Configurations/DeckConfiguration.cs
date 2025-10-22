using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeckConfiguration : IEntityTypeConfiguration<DeckEntity>
    {
        public void Configure(EntityTypeBuilder<DeckEntity> builder)
        {
            builder.HasKey(deck => deck.Id);
            builder.Property(deck => deck.Title)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(deck => deck.CourseName)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(deck => deck.SubjectName)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.HasOne(deck => deck.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(deck =>deck.FlashCards)
                .WithOne(fcard => fcard.Deck)
                .HasForeignKey(fcard => fcard.DeckId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}