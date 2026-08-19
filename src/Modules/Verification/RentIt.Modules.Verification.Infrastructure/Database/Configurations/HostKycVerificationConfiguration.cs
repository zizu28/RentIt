using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Verification.Domain.Entities;

namespace RentIt.Modules.Verification.Infrastructure.Database.Configurations;

internal sealed class HostKycVerificationConfiguration : IEntityTypeConfiguration<HostKycVerification>
{
    public void Configure(EntityTypeBuilder<HostKycVerification> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.HostId)
            .IsRequired();

        builder.Property(x => x.DocumentType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.EncryptedDocumentNumber)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.VerificationDate);
        
        builder.Property(x => x.Comments)
            .HasMaxLength(1000);
            
        builder.HasIndex(x => x.HostId);
    }
}
