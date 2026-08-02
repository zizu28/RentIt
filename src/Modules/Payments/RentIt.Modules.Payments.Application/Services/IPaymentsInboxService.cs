using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Payments.Application.Services;

public interface IPaymentsInboxService
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task InsertAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
