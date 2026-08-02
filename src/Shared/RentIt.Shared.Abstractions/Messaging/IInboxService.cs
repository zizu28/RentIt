using Microsoft.EntityFrameworkCore;

namespace RentIt.Shared.Abstractions.Messaging;

public interface IInboxService<TDbContext> where TDbContext : DbContext
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task InsertAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
