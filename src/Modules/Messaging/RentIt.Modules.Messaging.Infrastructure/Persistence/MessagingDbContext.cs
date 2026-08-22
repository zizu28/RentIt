using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Messaging.Domain.Entities;

using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence;

public class MessagingDbContext(DbContextOptions<MessagingDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<MessagingUser> Users { get; set; } = null!;
    public DbSet<Conversation> Conversations { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("messaging");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }
}
