using MediatR;

namespace RentIt.Modules.Payments.Application.Commands;

public record InitializePaymentCommand(
    Guid UserId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string Email,
    string CallbackUrl
) : IRequest<string>; // Returns the AuthorizationUrl
