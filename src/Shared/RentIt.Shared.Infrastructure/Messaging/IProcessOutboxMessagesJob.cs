using Microsoft.EntityFrameworkCore;

namespace RentIt.Shared.Infrastructure.Messaging;

public interface IProcessOutboxMessagesJob<TDbContext> where TDbContext : DbContext
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
