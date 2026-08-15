using MediatR;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class SetupPaymentMethodCommandHandler(IPaymentRepository paymentRepository) : IRequestHandler<SetupPaymentMethodCommand, string>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<string> Handle(SetupPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        // 1. Create a "Setup" payment (Amount = 0, BookingId = null)
        var setupPayment = Payment.CreateSetupPayment(request.UserId, request.Currency);
        
        // 2. Generate a mock Paystack authorization URL
        var authorizationUrl = $"https://checkout.paystack.com/{setupPayment.Reference}";
        setupPayment.SetAuthorizationUrl(authorizationUrl);

        // 3. Save to database
        await _paymentRepository.AddAsync(setupPayment, cancellationToken);
        // Note: the unit of work pipeline behavior or caller will call SaveChanges

        return authorizationUrl;
    }
}
