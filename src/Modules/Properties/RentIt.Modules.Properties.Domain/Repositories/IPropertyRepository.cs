using RentIt.Modules.Properties.Domain.Entities;

namespace RentIt.Modules.Properties.Domain.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);
    Task AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
}
