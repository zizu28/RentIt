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

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.BookingId)
            .IsRequired(false);

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

        builder.OwnsOne(x => x.Method, methodBuilder =>
        {
            methodBuilder.Property(m => m.Provider).HasMaxLength(50);
            methodBuilder.Property(m => m.MethodType).HasConversion<string>().HasMaxLength(20);
            methodBuilder.Property(m => m.Last4).HasMaxLength(4);
            methodBuilder.Property(m => m.EncryptedProviderToken).HasMaxLength(2000);
        });
            
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.UserId);
    }
}
