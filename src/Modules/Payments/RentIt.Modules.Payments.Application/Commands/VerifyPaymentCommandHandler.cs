using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Abstractions.Security;
using RentIt.Modules.Payments.Application.Services;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class VerifyPaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaystackService paystackService,
    [FromKeyedServices("Payments")] IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    Serilog.ILogger logger) : IRequestHandler<VerifyPaymentCommand, bool>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaystackService _paystackService = paystackService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<bool> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Reference))
        {
            return false;
        }

        var payment = await _paymentRepository.GetByReferenceAsync(request.Reference, cancellationToken);
        if (payment == null)
        {
            _logger.Warning("Payment with reference {Reference} not found during verification.", request.Reference);
            return false;
        }

        // If already processed, just return true
        if (payment.Status == PaymentStatus.Successful)
        {
            return true;
        }

        try
        {
            var verifyResponse = await _paystackService.VerifyTransactionAsync(request.Reference, cancellationToken);
            
            if (verifyResponse.Status && verifyResponse.Data != null)
            {
                var amount = verifyResponse.Data.Amount / 100m; // Paystack sends amount in kobo

                if (verifyResponse.Data.Status == "success")
                {
                    if (amount < payment.Amount)
                    {
                        payment.MarkAsPartiallyPaid(amount);
                    }
                    else
                    {
                        payment.MarkAsSuccessful();
                    }
                    
                    if (verifyResponse.Data.Authorization != null && !string.IsNullOrEmpty(verifyResponse.Data.Authorization.AuthorizationCode))
                    {
                        var encryptedToken = _encryptionService.Encrypt(verifyResponse.Data.Authorization.AuthorizationCode);
                        payment.SetProviderToken(encryptedToken);
                    }
                }
                else if (verifyResponse.Data.Status == "failed")
                {
                    payment.MarkAsFailed();
                }

                await _paymentRepository.UpdateAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                return payment.Status == PaymentStatus.Successful;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error verifying payment with reference {Reference}", request.Reference);
        }

        return false;
    }
}
