using MediatR;

namespace RentIt.Modules.Payments.Application.Queries;

public record GetPaymentByBookingIdQuery(Guid BookingId) : IRequest<PaymentDetailsDto>;

public record PaymentDetailsDto(Guid Id, Guid BookingId, string Reference, string Status, decimal Amount);
