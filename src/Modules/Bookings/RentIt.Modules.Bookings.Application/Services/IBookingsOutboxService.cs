using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Bookings.Application.Services;

public interface IBookingsOutboxService
{
    void Add(IIntegrationEvent integrationEvent);
}
