using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Infrastructure.Database;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Payments.Infrastructure.Services;

internal class PaymentsOutboxService(PaymentsDbContext dbContext) 
    : OutboxService<PaymentsDbContext>(dbContext), IPaymentsOutboxService
{
}
