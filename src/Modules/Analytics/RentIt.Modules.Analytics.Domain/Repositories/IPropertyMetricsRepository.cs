using RentIt.Modules.Analytics.Domain.Entities;

namespace RentIt.Modules.Analytics.Domain.Repositories;

public interface IPropertyMetricsRepository
{
    Task<PropertyMetrics?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task AddAsync(PropertyMetrics metrics, CancellationToken cancellationToken = default);
    void Update(PropertyMetrics metrics);
}
