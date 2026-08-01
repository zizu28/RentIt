using MediatR;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Payments.Application.Commands;

internal sealed class ProcessPaymentWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    IEventBus eventBus,
    IPaymentsUnitOfWork unitOfWork) : IRequestHandler<ProcessPaymentWebhookCommand>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IEventBus _eventBus = eventBus;
    private readonly IPaymentsUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        if (request.Payload.Event != "charge.success")
        {
            return; // We only care about successful charges for now
        }

        var reference = request.Payload.Data.Reference;
        if (string.IsNullOrEmpty(reference))
            return;

        var payment = await _paymentRepository.GetByReferenceAsync(reference, cancellationToken);
        if (payment == null)
            return;

        if (payment.Status == Domain.Enums.PaymentStatus.Successful)
            return; // Already processed

        // Update local state
        payment.MarkAsSuccessful();
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish integration event to notify Bookings module
        // RenterId is not stored in Payment currently, but we can pass Guid.Empty or store it in Payment. 
        // Ideally, Payment should store RenterId if required by the event.
        // Let's publish it with Guid.Empty for RenterId since the Bookings module mainly needs BookingId.
        var integrationEvent = new PaymentCompletedIntegrationEvent(
            payment.Id,
            payment.BookingId,
            Guid.Empty, // RenterId not tracked in Payment module directly
            payment.Amount,
            payment.Currency,
            "Paystack"
        );

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
