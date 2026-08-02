using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Bookings.Application.Services;

public interface IBookingsInboxService
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task InsertAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
