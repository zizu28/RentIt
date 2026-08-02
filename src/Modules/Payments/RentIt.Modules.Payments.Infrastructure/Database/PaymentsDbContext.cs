using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Payments.Infrastructure.Database;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
