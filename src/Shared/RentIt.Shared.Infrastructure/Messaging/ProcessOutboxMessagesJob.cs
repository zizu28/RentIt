using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

public class ProcessOutboxMessagesJob<TDbContext>(
    TDbContext dbContext,
    IPublisher publisher) : IProcessOutboxMessagesJob<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (!messages.Any())
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == message.Type);

                if (type == null)
                {
                    message.MarkAsFailed($"Type {message.Type} not found.");
                    continue;
                }

                var integrationEvent = JsonConvert.DeserializeObject(message.Content, type) as IIntegrationEvent;

                if (integrationEvent == null)
                {
                    message.MarkAsFailed($"Failed to deserialize to IIntegrationEvent.");
                    continue;
                }

                await _publisher.Publish(integrationEvent, cancellationToken);
                message.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.Message);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
