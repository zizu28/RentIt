using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Enums;

namespace RentIt.Modules.Payments.Infrastructure.Database.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.AmountPaid)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.HasIndex(x => x.Reference)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PaymentStatus>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AuthorizationUrl)
            .HasMaxLength(500);
            
        builder.HasIndex(x => x.BookingId);
    }
}
