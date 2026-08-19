using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;

namespace RentIt.Modules.Analytics.Infrastructure.Repositories;

internal sealed class PropertyMetricsRepository(Database.AnalyticsDbContext dbContext) : IPropertyMetricsRepository
{
    private readonly Database.AnalyticsDbContext _dbContext = dbContext;

    public async Task<PropertyMetrics?> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PropertyMetrics
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId, cancellationToken);
    }

    public async Task AddAsync(PropertyMetrics metrics, CancellationToken cancellationToken = default)
    {
        await _dbContext.PropertyMetrics.AddAsync(metrics, cancellationToken);
    }

    public void Update(PropertyMetrics metrics)
    {
        _dbContext.PropertyMetrics.Update(metrics);
    }
}
