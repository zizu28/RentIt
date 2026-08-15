using MediatR;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

public record PaymentInitializationFailedIntegrationEvent(Guid BookingId, string Reason) : INotification;
