using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class InitializePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaystackService paystackService,
    [FromKeyedServices("Payments")] IUnitOfWork unitOfWork,
    Serilog.ILogger logger)
    : IRequestHandler<InitializePaymentCommand, string>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaystackService _paystackService = paystackService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<string> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Initializing payment for Booking {BookingId}", request.BookingId);
        
        // 1. Check if a payment already exists for this booking and is pending
        var existingPayment = await _paymentRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existingPayment != null && existingPayment.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(existingPayment.AuthorizationUrl))
        {
            _logger.Information("Found existing pending payment with Auth URL for Booking {BookingId}", request.BookingId);
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

        _logger.Information("Successfully initialized payment for Booking {BookingId} with Reference {Reference}", request.BookingId, payment.Reference);
        return response.Data.AuthorizationUrl;
    }
}
