using MediatR;

namespace RentIt.Modules.Payments.Application.Commands;

public record VerifyPaymentCommand(string Reference) : IRequest<bool>;
