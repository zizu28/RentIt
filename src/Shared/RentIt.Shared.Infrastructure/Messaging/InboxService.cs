using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

public class InboxService<TDbContext>(TDbContext dbContext) : IInboxService<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext = dbContext;

    public Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<InboxMessage>().AnyAsync(m => m.Id == eventId, cancellationToken);
    }

    public async Task InsertAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.EventId,
            Type = integrationEvent.GetType().FullName!,
            Content = JsonConvert.SerializeObject(integrationEvent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }),
            OccurredOn = DateTime.UtcNow
        };
        inboxMessage.MarkAsProcessed();

        await _dbContext.Set<InboxMessage>().AddAsync(inboxMessage, cancellationToken);
    }
}
