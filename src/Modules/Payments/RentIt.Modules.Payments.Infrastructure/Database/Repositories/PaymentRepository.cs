using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Payments.Domain.Entities;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Infrastructure.Database.Repositories;

internal sealed class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    private readonly PaymentsDbContext _context = dbContext;

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Reference == reference, cancellationToken);
    }

    public async Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(payment);
        return Task.CompletedTask;
    }
}
