﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFox.Core.Aggregates;
using MoneyFox.Core.Aggregates.Payments;

namespace MoneyFox.Infrastructure.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Set FK from category to payment with cascade with cascade
            builder
                .HasMany(m => m.Payments)
                .WithOne(t => t.Category!)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(b => b.Name);
        }
    }
}