using RentIt.Modules.Bookings.Domain.Entities;

namespace RentIt.Modules.Bookings.Domain.Repositories;

public interface IBookablePropertyRepository
{
    Task<BookableProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookableProperty>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);
    void Add(BookableProperty property);
    void Update(BookableProperty property);
    void Remove(BookableProperty property);
}
