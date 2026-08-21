using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Modules.Analytics.Infrastructure.Database;

namespace RentIt.Modules.Analytics.Infrastructure.Repositories;

internal sealed class HostMetricsRepository : IHostMetricsRepository
{
    private readonly AnalyticsDbContext _context;

    public HostMetricsRepository(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<HostMetrics?> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
    {
        return await _context.HostMetrics
            .FirstOrDefaultAsync(x => x.HostId == hostId, cancellationToken);
    }

    public async Task AddAsync(HostMetrics metrics, CancellationToken cancellationToken = default)
    {
        await _context.HostMetrics.AddAsync(metrics, cancellationToken);
    }

    public void Update(HostMetrics metrics)
    {
        _context.HostMetrics.Update(metrics);
    }
}
