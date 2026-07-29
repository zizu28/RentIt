using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Properties.Domain.Entities;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Modules.Properties.Infrastructure.Database;

namespace RentIt.Modules.Properties.Infrastructure.Repositories;

internal class PropertyRepository(PropertiesDbContext dbContext) : IPropertyRepository
{
    private readonly PropertiesDbContext _dbContext = dbContext;

    public async Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .Where(p => p.HostId == hostId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _dbContext.Properties.AddAsync(property, cancellationToken);
    }

    public Task UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        _dbContext.Properties.Update(property);
        return Task.CompletedTask;
    }
}
