using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Verification.Domain.Entities;

namespace RentIt.Modules.Verification.Infrastructure.Database;

public class VerificationDbContext(DbContextOptions<VerificationDbContext> options) : DbContext(options)
{
    public DbSet<HostKycVerification> HostKycVerifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("verification");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VerificationDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new RentIt.Shared.Infrastructure.Messaging.OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new RentIt.Shared.Infrastructure.Messaging.InboxMessageConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
