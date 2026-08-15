using MediatR;

namespace RentIt.Modules.Payments.Application.Commands;

public record SetupPaymentMethodCommand(Guid UserId, string Currency) : IRequest<string>;
