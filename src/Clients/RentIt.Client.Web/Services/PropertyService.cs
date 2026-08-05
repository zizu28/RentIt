using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Client.Web.Services;

public class PropertyService(HttpClient httpClient) : IPropertyService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Guid> CreatePropertyAsync(CreatePropertyRequest request, IEnumerable<IBrowserFile> images)
    {
        using var content = new MultipartFormDataContent();

        // Add text fields
        content.Add(new StringContent(request.Name ?? string.Empty), "Name");
        content.Add(new StringContent(request.Description ?? string.Empty), "Description");
        content.Add(new StringContent(request.Street ?? string.Empty), "Street");
        content.Add(new StringContent(request.City ?? string.Empty), "City");
        content.Add(new StringContent(request.Region ?? string.Empty), "Region");
        content.Add(new StringContent(request.Country ?? string.Empty), "Country");
        content.Add(new StringContent(request.PostalCode ?? string.Empty), "PostalCode");
        content.Add(new StringContent(request.Type.ToString()), "Type");
        content.Add(new StringContent(request.Status.ToString()), "Status");
        content.Add(new StringContent(request.RentalPeriod.ToString()), "RentalPeriod");
        content.Add(new StringContent(request.PricePerPeriod.ToString()), "PricePerPeriod");
        content.Add(new StringContent(request.Bedrooms.ToString()), "Bedrooms");
        content.Add(new StringContent(request.Bathrooms.ToString()), "Bathrooms");

        if (request.Amenities != null)
        {
            foreach (var amenity in request.Amenities)
            {
                content.Add(new StringContent(amenity), "Amenities");
            }
        }

        // Add files
        var streamContents = new List<StreamContent>();
        try
        {
            if (images != null)
            {
                foreach (var file in images)
                {
                    // Max allowed size is set to 5MB here for testing
                    var streamContent = new StreamContent(file.OpenReadStream(1024 * 1024 * 5));
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(streamContent, "Images", file.Name);
                    streamContents.Add(streamContent);
                }
            }

            var response = await _httpClient.PostAsync("api/properties", content);
            //if(request.Status == 2)
            //{
            //    var bookableResponse = await _httpClient.PostAsJsonAsync("", content);
            //}
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Guid>();
        }
        finally
        {
            // Ensure streams are disposed
            foreach (var sc in streamContents)
            {
                sc.Dispose();
            }
        }
    }

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<PropertyDto>>("api/properties") ?? Enumerable.Empty<PropertyDto>();
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<PropertyDto>($"api/properties/{id}");
    }

    public async Task<IEnumerable<PropertyDto>> GetHostPropertiesAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<PropertyDto>>("api/properties/host") ?? Enumerable.Empty<PropertyDto>();
    }

    public async Task UpdatePropertyAsync(Guid id, UpdatePropertyRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/properties/{id}", request);
        response.EnsureSuccessStatusCode();
    }
}
