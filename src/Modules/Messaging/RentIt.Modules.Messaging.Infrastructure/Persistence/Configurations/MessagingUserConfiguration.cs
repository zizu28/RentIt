using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence.Configurations;

internal sealed class MessagingUserConfiguration : IEntityTypeConfiguration<MessagingUser>
{
    public void Configure(EntityTypeBuilder<MessagingUser> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
