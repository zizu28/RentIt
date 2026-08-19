using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Reviews.Domain.Entities;

namespace RentIt.Modules.Reviews.Infrastructure.Database.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PropertyId)
            .IsRequired();

        builder.Property(r => r.GuestId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.PropertyId);
        builder.HasIndex(r => r.GuestId);
    }
}
