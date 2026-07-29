namespace RentIt.Shared.Abstractions.Storage;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}
