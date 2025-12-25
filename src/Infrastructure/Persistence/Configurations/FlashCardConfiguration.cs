﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class FlashCardConfiguration :IEntityTypeConfiguration<FlashCardEntity>
    {
        public void Configure(EntityTypeBuilder<FlashCardEntity> builder)
        {
            builder.HasKey(fcard => fcard.Id);

            builder.Property(fcard => fcard.Id);
            builder.Property(fcard => fcard.FrontText)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(fcard => fcard.BackText)
                .IsRequired();
            
            builder.HasIndex(fcard => fcard.NextReviewAtUtc);
        }
    }
}