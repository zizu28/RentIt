using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.ValueObjects;

namespace RentIt.Modules.Identity.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "identity");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                phone => phone.Value,
                value => PhoneNumber.Create(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => PasswordHash.Create(value))
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasMaxLength(100);

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(1000);

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany<RefreshToken>()
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.Property(u => u.VerificationToken).HasMaxLength(100);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(100);
        builder.Property(u => u.PasswordResetTokenExpiresAt);

        builder.Property(u => u.RowVersion)
            .IsRowVersion();

        builder.Ignore(u => u.DomainEvents);
        builder.Ignore(u => u.FullName);
        
        builder.Metadata.FindNavigation(nameof(User.RefreshTokens))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
