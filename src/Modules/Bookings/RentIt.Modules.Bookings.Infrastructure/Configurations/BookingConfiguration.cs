using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Enums;

namespace RentIt.Modules.Bookings.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.PropertyId).IsRequired();
        builder.Property(b => b.GuestId).IsRequired();
        builder.Property(b => b.StartDate).IsRequired();
        builder.Property(b => b.EndDate).IsRequired();
        
        builder.OwnsOne(b => b.TotalPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("TotalPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            price.Property(p => p.Currency)
                .HasConversion<string>()
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(b => b.Status)
            .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<BookingStatus>(v, true))
            .HasMaxLength(20);
    }
}
