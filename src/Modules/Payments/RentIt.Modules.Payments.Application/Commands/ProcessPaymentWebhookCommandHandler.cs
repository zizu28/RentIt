using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Payments.Domain.Enums;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Abstractions.Security;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class ProcessPaymentWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    [FromKeyedServices("Payments")] IUnitOfWork unitOfWork,
    IEncryptionService encryptionService) : IRequestHandler<ProcessPaymentWebhookCommand>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var eventType = request.Payload.Event;
        var reference = request.Payload.Data.Reference;
        var amount = request.Payload.Data.Amount / 100m; // Paystack sends amount in kobo

        if (string.IsNullOrEmpty(reference))
        {
            return;
        }

        var payment = await _paymentRepository.GetByReferenceAsync(reference, cancellationToken);
        if (payment == null)
        {
            return;
        }

        switch (eventType)
        {
            case "charge.success":
                if (payment.Status == PaymentStatus.Successful)
                {
                    return; // Already processed
                }

                if (amount < payment.Amount)
                {
                    payment.MarkAsPartiallyPaid(amount);
                }
                else
                {
                    payment.MarkAsSuccessful();
                }
                
                if (request.Payload.Data.Authorization != null && !string.IsNullOrEmpty(request.Payload.Data.Authorization.AuthorizationCode))
                {
                    var encryptedToken = _encryptionService.Encrypt(request.Payload.Data.Authorization.AuthorizationCode);
                    payment.SetProviderToken(encryptedToken);
                }
                break;

            case "charge.failed":
                if (payment.Status == PaymentStatus.Failed)
                {
                    return;
                }
                payment.MarkAsFailed();
                break;

            case "refund.processed":
                if (payment.Status == PaymentStatus.Refunded)
                {
                    return;
                }
                payment.MarkAsRefunded();
                break;
            default:
                return; // Ignored events
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        // This will save changes AND dispatch domain events
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
