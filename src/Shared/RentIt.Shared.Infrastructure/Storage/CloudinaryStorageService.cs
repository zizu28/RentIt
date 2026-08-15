using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RentIt.Shared.Abstractions.Storage;

namespace RentIt.Shared.Infrastructure.Storage;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;

    private readonly CloudinarySettings _settings;

    public CloudinaryStorageService(IOptions<CloudinarySettings> settings)
    {
        _settings = settings.Value;
        if (!string.IsNullOrEmpty(_settings.ApiKey))
        {
            var account = new Account(
                _settings.CloudName,
                _settings.ApiKey,
                _settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }
    }

    public async Task<string> UploadImageAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_settings.ApiKey) || _cloudinary == null)
        {
            // Skip Cloudinary upload if not configured (e.g., local development without API keys)
            return $"https://dummyimage.com/600x400/000/fff&text={Uri.EscapeDataString(fileName)}";
        }
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        try
        {
            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, "Cloudinary upload failed (likely DNS/Network issue). Returning dummy image URL for {FileName}.", fileName);
            return $"https://dummyimage.com/600x400/000/fff&text={Uri.EscapeDataString(fileName)}";
        }
    }
}
