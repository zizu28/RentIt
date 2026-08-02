using Microsoft.EntityFrameworkCore;

namespace RentIt.Shared.Abstractions.Messaging;

public interface IOutboxService<TDbContext> where TDbContext : DbContext
{
    void Add(IIntegrationEvent integrationEvent);
}
