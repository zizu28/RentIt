using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Infrastructure.Database.Repositories;

internal sealed class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    private readonly PaymentsDbContext _dbContext = dbContext;

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
    }

    public async Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId, cancellationToken);
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Payments.AddAsync(payment, cancellationToken);
    }

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _dbContext.Payments.Update(payment);
        return Task.CompletedTask;
    }
}
