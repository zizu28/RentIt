using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

public class OutboxService<TDbContext>(TDbContext dbContext) : IOutboxService<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext = dbContext;

    public void Add(IIntegrationEvent integrationEvent)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = integrationEvent.GetType().FullName!,
            Content = JsonConvert.SerializeObject(integrationEvent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }),
            OccurredOn = DateTime.UtcNow
        };

        _dbContext.Set<OutboxMessage>().Add(outboxMessage);
    }
}
