using RentIt.Modules.Analytics.Domain.Entities;

namespace RentIt.Modules.Analytics.Domain.Repositories;

public interface IHostMetricsRepository
{
    Task<HostMetrics?> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);
    Task AddAsync(HostMetrics metrics, CancellationToken cancellationToken = default);
    void Update(HostMetrics metrics);
}
