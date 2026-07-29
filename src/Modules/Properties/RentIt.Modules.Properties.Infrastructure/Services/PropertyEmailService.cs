using RentIt.Modules.Properties.Application.Services;
using RentIt.Shared.Abstractions.Email;
using Serilog;

namespace RentIt.Modules.Properties.Infrastructure.Services;

internal sealed class PropertyEmailService(Serilog.ILogger logger, IEmailService emailService) : IPropertyEmailService
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IEmailService _emailService = emailService;

    public async Task SendPropertyCreationEmailAsync(Guid hostId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        _logger.Information("Sending Property Creation Email to Host: {HostId} for Property: {PropertyId}", hostId, propertyId);

        // In a real scenario, we would retrieve the host's email address using the hostId.
        // For demonstration with Mailtrap, we'll send it to a test email address.
        var hostEmail = "host@rentit.com"; 
        
        var subject = $"Your Property {propertyId} has been successfully created!";
        var body = $"Hello, \n\nYour new property with ID {propertyId} was successfully created on RentIt. It will be available for rent shortly.";
        
        await _emailService.SendEmailAsync(hostEmail, subject, body, cancellationToken);
        
        _logger.Information("Successfully sent Property Creation Email for Property: {PropertyId}", propertyId);
    }
}
