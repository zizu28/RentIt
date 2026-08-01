using MediatR;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class ProcessPaymentWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    IPaymentsUnitOfWork unitOfWork) : IRequestHandler<ProcessPaymentWebhookCommand>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaymentsUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var eventType = request.Payload.Event;
        var reference = request.Payload.Data.Reference;
        var amount = request.Payload.Data.Amount / 100m; // Paystack sends amount in kobo

        if (string.IsNullOrEmpty(reference))
            return;

        var payment = await _paymentRepository.GetByReferenceAsync(reference, cancellationToken);
        if (payment == null)
            return;

        switch (eventType)
        {
            case "charge.success":
                if (payment.Status == Domain.Enums.PaymentStatus.Successful)
                    return; // Already processed

                if (amount < payment.Amount)
                {
                    payment.MarkAsPartiallyPaid(amount);
                }
                else
                {
                    payment.MarkAsSuccessful();
                }
                break;

            case "charge.failed":
                if (payment.Status == Domain.Enums.PaymentStatus.Failed)
                    return;
                payment.MarkAsFailed();
                break;

            case "refund.processed":
                if (payment.Status == Domain.Enums.PaymentStatus.Refunded)
                    return;
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
