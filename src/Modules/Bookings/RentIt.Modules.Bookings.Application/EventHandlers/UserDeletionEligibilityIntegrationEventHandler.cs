using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

internal sealed class UserDeletionEligibilityIntegrationEventHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository
) : INotificationHandler<UserDeletionEligibilityIntegrationEvent>
{
    public async Task Handle(UserDeletionEligibilityIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Role == "Renter")
        {
            var guestBookings = await bookingRepository.GetByGuestIdAsync(notification.UserId, cancellationToken);
            
            // Check if Renter has active or upcoming bookings
            var hasUpcoming = guestBookings.Any(b => 
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) && 
                b.StartDate >= DateOnly.FromDateTime(DateTime.UtcNow));

            if (hasUpcoming)
            {
                notification.Context.Reject("Guest has pending or upcoming bookings.");
            }
        }
        else if (notification.Role == "Host")
        {
            var properties = await propertyRepository.GetByHostIdAsync(notification.UserId, cancellationToken);
            if (properties.Any())
            {
                var propertyIds = properties.Select(p => p.Id).ToList();
                var pendingBookings = await bookingRepository.GetPendingBookingsByPropertyIdsAsync(propertyIds, cancellationToken);
                
                var hasUpcoming = pendingBookings.Any(b => b.StartDate >= DateOnly.FromDateTime(DateTime.UtcNow));
                
                if (hasUpcoming)
                {
                    notification.Context.Reject("Host has pending or upcoming bookings for their properties.");
                }
            }
        }
    }
}
