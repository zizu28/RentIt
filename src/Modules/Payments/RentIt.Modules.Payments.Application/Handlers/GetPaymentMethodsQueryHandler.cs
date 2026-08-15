using MediatR;
using RentIt.Modules.Payments.Application.Queries;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Application.Handlers;

internal sealed class GetPaymentMethodsQueryHandler(IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentMethodsQuery, List<PaymentMethodDto>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<List<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var paymentMethods = payments
            .Where(p => p.Method != null)
            .Select(p => p.Method)
            .DistinctBy(m => m!.EncryptedProviderToken) // Use token as distinct identifier
            .ToList();

        return paymentMethods.Select(m => new PaymentMethodDto(
            m!.Provider,
            m.MethodType.ToString(),
            m.Last4,
            m.ExpiryMonth,
            m.ExpiryYear
        )).ToList();
    }
}

