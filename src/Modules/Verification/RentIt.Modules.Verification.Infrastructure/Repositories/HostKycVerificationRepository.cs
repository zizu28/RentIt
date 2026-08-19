using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Verification.Domain.Entities;
using RentIt.Modules.Verification.Domain.Repositories;
using RentIt.Modules.Verification.Infrastructure.Database;

namespace RentIt.Modules.Verification.Infrastructure.Repositories;

internal sealed class HostKycVerificationRepository(VerificationDbContext dbContext) : IHostKycVerificationRepository
{
    private readonly VerificationDbContext _dbContext = dbContext;

    public async Task<HostKycVerification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HostKycVerifications
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<HostKycVerification?> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HostKycVerifications
            .Where(v => v.HostId == hostId)
            .OrderByDescending(v => v.VerificationDate ?? DateTime.MaxValue)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(HostKycVerification verification, CancellationToken cancellationToken = default)
    {
        await _dbContext.HostKycVerifications.AddAsync(verification, cancellationToken);
    }

    public void Update(HostKycVerification verification)
    {
        _dbContext.HostKycVerifications.Update(verification);
    }
}
