using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Infrastructure.Messaging;
using RentIt.Modules.Identity.Infrastructure.Persistence;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Identity.Infrastructure;

/// <summary>
/// Registers Identity module infrastructure services.
/// Wires up the choreography pipeline:
///   AggregateRoot → DomainEvent → MediatR → DomainEventHandler → IEventBus → IntegrationEvent → Consumers
/// </summary>
public static class IdentityInfrastructureServiceRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        // Domain event dispatcher — collects events from aggregates and publishes via MediatR
        services.AddScoped<DomainEventDispatcher>();

        // Unit of work — dispatches domain events after SaveChangesAsync
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Event bus — dispatches integration events to handlers (in-memory for modular monolith)
        services.AddScoped<IEventBus, InMemoryEventBus>();

        return services;
    }
}
