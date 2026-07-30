using RentIt.Shared.DTOs.Properties;

namespace RentIt.Client.Web.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PropertyDto?> GetPropertyByIdAsync(Guid id);
    Task<IEnumerable<PropertyDto>> GetHostPropertiesAsync();
    Task<Guid> CreatePropertyAsync(CreatePropertyRequest request, IEnumerable<Microsoft.AspNetCore.Components.Forms.IBrowserFile> images);
    Task UpdatePropertyAsync(Guid id, UpdatePropertyRequest request);
}
