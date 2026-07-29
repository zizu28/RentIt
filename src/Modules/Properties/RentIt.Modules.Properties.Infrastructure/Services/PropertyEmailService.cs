using RentIt.Modules.Properties.Application.Services;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentIt.Modules.Properties.Infrastructure.Services;

internal sealed class PropertyEmailService : IPropertyEmailService
{
    private readonly ILogger _logger;

    public PropertyEmailService(ILogger logger)
    {
        _logger = logger;
    }

    public Task SendPropertyCreationEmailAsync(Guid hostId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        // TODO: Integrate with real email service
        _logger.Information("Sending Property Creation Email to Host: {HostId} for Property: {PropertyId}", hostId, propertyId);
        
        return Task.CompletedTask;
    }
}
