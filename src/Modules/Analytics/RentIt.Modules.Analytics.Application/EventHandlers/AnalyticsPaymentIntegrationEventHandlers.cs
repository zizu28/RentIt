using MediatR;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class AnalyticsPaymentIntegrationEventHandlers(ILogger logger) :
    INotificationHandler<PaymentCompletedIntegrationEvent>,
    INotificationHandler<PaymentFailedIntegrationEvent>
{
    private readonly ILogger _logger = logger;

    public Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Analytics: Recorded payment completion for {Amount} {Currency} on Payment {PaymentId}",
            notification.Amount, notification.Currency, notification.PaymentId);
        
        return Task.CompletedTask;
    }

    public Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.Warning("Analytics: Recorded payment failure for {Reason} on Payment {PaymentId}",
            notification.Reason, notification.PaymentId);
        
        return Task.CompletedTask;
    }
}
