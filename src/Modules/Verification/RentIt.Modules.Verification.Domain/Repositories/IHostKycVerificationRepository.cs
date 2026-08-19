using RentIt.Modules.Verification.Domain.Entities;

namespace RentIt.Modules.Verification.Domain.Repositories;

public interface IHostKycVerificationRepository
{
    Task<HostKycVerification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HostKycVerification?> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default);
    Task AddAsync(HostKycVerification verification, CancellationToken cancellationToken = default);
    void Update(HostKycVerification verification);
}
