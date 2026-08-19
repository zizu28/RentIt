using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Reviews.Domain.Entities;
using RentIt.Modules.Reviews.Domain.Repositories;
using RentIt.Modules.Reviews.Infrastructure.Database;

namespace RentIt.Modules.Reviews.Infrastructure.Repositories;

internal sealed class ReviewRepository(ReviewsDbContext dbContext) : IReviewRepository
{
    private readonly ReviewsDbContext _dbContext = dbContext;

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByGuestIdAsync(Guid guestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .Where(r => r.GuestId == guestId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reviews.AddAsync(review, cancellationToken);
    }

    public void Update(Review review)
    {
        _dbContext.Reviews.Update(review);
    }

    public void Delete(Review review)
    {
        _dbContext.Reviews.Remove(review);
    }
}
