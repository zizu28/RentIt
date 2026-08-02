using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Payments.Application.Services;

public interface IPaymentsOutboxService
{
    void Add(IIntegrationEvent integrationEvent);
}
