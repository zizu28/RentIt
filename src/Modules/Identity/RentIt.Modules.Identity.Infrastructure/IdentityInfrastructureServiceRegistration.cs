using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Infrastructure.Messaging;
using RentIt.Modules.Identity.Infrastructure.Persistence;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Infrastructure.Services;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Infrastructure.Repositories;
using RentIt.Modules.Identity.Infrastructure.Services.SocialAuth;

namespace RentIt.Modules.Identity.Infrastructure;

/// <summary>
/// Registers Identity module infrastructure services.
/// Wires up the choreography pipeline:
///   AggregateRoot → DomainEvent → MediatR → DomainEventHandler → IEventBus → IntegrationEvent → Consumers
/// </summary>
public static class IdentityInfrastructureServiceRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityDatabase")));

        // Domain event dispatcher — collects events from aggregates and publishes via MediatR
        services.AddScoped<DomainEventDispatcher>();

        // Unit of work — dispatches domain events after SaveChangesAsync
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Event bus — dispatches integration events to handlers (in-memory for modular monolith)
        services.AddScoped<IEventBus, InMemoryEventBus>();

        // Social Authentication Services
        services.AddHttpClient<FacebookAuthService>();
        services.AddHttpClient<GoogleAuthService>();
        services.AddHttpClient<MicrosoftAuthService>();
        services.AddScoped<ISocialAuthServiceFactory, SocialAuthServiceFactory>();

        return services;
    }
}
