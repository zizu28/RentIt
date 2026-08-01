using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Bookings.Domain.Entities;

namespace RentIt.Modules.Bookings.Infrastructure.Configurations;

public class BookablePropertyConfiguration : IEntityTypeConfiguration<BookableProperty>
{
    public void Configure(EntityTypeBuilder<BookableProperty> builder)
    {
        builder.HasKey(p => p.Id);
        
        // This id is supplied by the Properties module
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Currency).HasMaxLength(3).IsRequired();
    }
}
