using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Infrastructure.Persistence;

namespace RentIt.Modules.Identity.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(IdentityDbContext dbContext) : IRefreshTokenRepository
{
    private readonly IdentityDbContext _dbContext = dbContext;

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public void Update(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
    }
}
