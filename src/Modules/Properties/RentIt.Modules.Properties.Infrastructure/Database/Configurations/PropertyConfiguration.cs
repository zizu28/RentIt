using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Properties.Domain.Entities;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Kernel.Enums;
using System.Text.Json;

namespace RentIt.Modules.Properties.Infrastructure.Database.Configurations;

internal class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.HostId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PropertyStatus>(v, true))
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Address_Street").HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasColumnName("Address_City").HasMaxLength(100).IsRequired();
            address.Property(a => a.Region).HasColumnName("Address_Region").HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("Address_PostalCode").HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasColumnName("Address_Country").HasMaxLength(100).IsRequired();
        });

        builder.OwnsOne(x => x.PricePerPeriod, price =>
        {
            price.Property(p => p.Amount)
            .HasColumnName("Price_Amount")
            .HasColumnType("decimal(18,2)").IsRequired();
            price.Property(p => p.Currency)
                .HasColumnName("Price_Currency")
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Currency>(v, true))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(x => x.SecurityDeposit, deposit =>
        {
            deposit.Property(p => p.Amount)
            .HasColumnName("SecurityDeposit_Amount")
            .HasColumnType("decimal(18,2)").IsRequired();
            deposit.Property(p => p.Currency)
                .HasColumnName("SecurityDeposit_Currency")
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Currency>(v, true))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Amenities)
            .HasField("_amenities")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Images)
            .HasField("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");
    }
}
