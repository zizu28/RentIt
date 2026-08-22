using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class InitializePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaystackService paystackService,
    [FromKeyedServices("Payments")] IUnitOfWork unitOfWork,
    Serilog.ILogger logger,
    IPublisher publisher)
    : IRequestHandler<InitializePaymentCommand, string>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaystackService _paystackService = paystackService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;
    private readonly IPublisher _publisher = publisher;

    public async Task<string> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Initializing payment for Booking {BookingId}", request.BookingId);
        
        try
        {
            // 1. Check if a payment already exists for this booking
            var existingPayment = await _paymentRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
            if (existingPayment != null)
            {
                if (existingPayment.Status == PaymentStatus.Successful)
                {
                    return $"{request.CallbackUrl}?reference={existingPayment.Reference}";
                }

                if (existingPayment.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(existingPayment.AuthorizationUrl))
                {
                    _logger.Information("Found existing pending payment {Reference} for Booking {BookingId}. Verifying status...", existingPayment.Reference, request.BookingId);
                    try
                    {
                        var verifyResponse = await _paystackService.VerifyTransactionAsync(existingPayment.Reference, cancellationToken);
                        if (verifyResponse.Status && verifyResponse.Data != null && verifyResponse.Data.Status == "success")
                        {
                            return $"{request.CallbackUrl}?reference={existingPayment.Reference}";
                        }
                        
                        // Abandoned or failed, mark as failed so we can create a new one
                        existingPayment.MarkAsFailed();
                        await _paymentRepository.UpdateAsync(existingPayment, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Failed to verify existing payment {Reference}. Marking as failed and generating new one.", existingPayment.Reference);
                        existingPayment.MarkAsFailed();
                        await _paymentRepository.UpdateAsync(existingPayment, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            // 2. Create the Payment entity
            var payment = Payment.Create(request.UserId, request.BookingId, request.Amount, request.Currency);
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
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to initialize payment for Booking {BookingId}. Triggering compensation.", request.BookingId);
            
            // Trigger compensating transaction via integration event
            await _publisher.Publish(new RentIt.Shared.Contracts.Payments.IntegrationEvents.PaymentInitializationFailedIntegrationEvent(request.BookingId, ex.Message), cancellationToken);
            
            throw; // Rethrow to propagate failure
        }
    }
}
