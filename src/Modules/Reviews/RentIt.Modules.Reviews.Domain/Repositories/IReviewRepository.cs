using RentIt.Modules.Reviews.Domain.Entities;

namespace RentIt.Modules.Reviews.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByGuestIdAsync(Guid guestId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);
    void Delete(Review review);
}
