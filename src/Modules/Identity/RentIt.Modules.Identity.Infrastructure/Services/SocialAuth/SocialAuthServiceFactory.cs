using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Application.Abstractions;

namespace RentIt.Modules.Identity.Infrastructure.Services.SocialAuth;

public class SocialAuthServiceFactory(IServiceProvider serviceProvider) : ISocialAuthServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISocialAuthService Create(string provider)
    {
        return provider.ToLowerInvariant() switch
        {
            "facebook" => _serviceProvider.GetRequiredService<FacebookAuthService>(),
            // "google" => _serviceProvider.GetRequiredService<GoogleAuthService>(), // Add when implementing Google
            _ => throw new NotSupportedException($"Social authentication provider '{provider}' is not supported.")
        };
    }
}
