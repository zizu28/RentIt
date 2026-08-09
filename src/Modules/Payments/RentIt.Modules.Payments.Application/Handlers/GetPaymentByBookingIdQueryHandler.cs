using MediatR;
using RentIt.Modules.Payments.Application.Queries;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Application.Handlers;

public class GetPaymentByBookingIdQueryHandler(IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentByBookingIdQuery, PaymentDetailsDto?>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<PaymentDetailsDto?> Handle(GetPaymentByBookingIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        
        if (payment == null) return null;

        return new PaymentDetailsDto(
            payment.Id,
            payment.BookingId,
            payment.Reference,
            payment.Status.ToString(),
            payment.Amount
        );
    }
}
