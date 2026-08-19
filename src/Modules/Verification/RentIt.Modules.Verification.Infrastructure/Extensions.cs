using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Verification.Domain.Repositories;
using RentIt.Modules.Verification.Infrastructure.Database;
using RentIt.Modules.Verification.Infrastructure.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Verification.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddVerificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VerificationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("VerificationDatabase"));
        });

        services.AddScoped<IHostKycVerificationRepository, HostKycVerificationRepository>();
        services.AddKeyedScoped<IUnitOfWork, VerificationUnitOfWork>("Verification");

        return services;
    }
}
