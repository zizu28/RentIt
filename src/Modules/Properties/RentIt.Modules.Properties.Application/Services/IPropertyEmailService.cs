namespace RentIt.Modules.Properties.Application.Services;

public interface IPropertyEmailService
{
    Task SendPropertyCreationEmailAsync(Guid hostId, Guid propertyId, CancellationToken cancellationToken = default);
}
