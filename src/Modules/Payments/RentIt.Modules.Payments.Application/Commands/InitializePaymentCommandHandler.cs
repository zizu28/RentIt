using MediatR;
using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class InitializePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaystackService paystackService,
    IPaymentsUnitOfWork unitOfWork)
    : IRequestHandler<InitializePaymentCommand, string>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaystackService _paystackService = paystackService;
    private readonly IPaymentsUnitOfWork _unitOfWork = unitOfWork;

    public async Task<string> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if a payment already exists for this booking and is pending
        var existingPayment = await _paymentRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existingPayment != null && existingPayment.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(existingPayment.AuthorizationUrl))
        {
            return existingPayment.AuthorizationUrl; // Return existing URL
        }

        // 2. Create the Payment entity
        var payment = Payment.Create(request.BookingId, request.Amount, request.Currency);
        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Initialize with Paystack
        var paystackRequest = new InitializeTransactionRequest
        {
            Email = request.Email,
            Amount = request.Amount,
            Reference = payment.Reference,
            CallbackUrl = request.CallbackUrl
        };

        var response = await _paystackService.InitializeTransactionAsync(paystackRequest, cancellationToken);

        // 4. Update payment with Auth URL
        payment.SetAuthorizationUrl(response.Data.AuthorizationUrl);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response.Data.AuthorizationUrl;
    }
}
