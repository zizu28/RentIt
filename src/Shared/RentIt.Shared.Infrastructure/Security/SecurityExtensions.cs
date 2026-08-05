using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Security;

namespace RentIt.Shared.Infrastructure.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddSharedSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();
        return services;
    }
}
