namespace RentIt.Shared.Abstractions.Pdf;

public interface IPdfService
{
    byte[] GeneratePdf<TModel>(TModel model);
    Task<byte[]> GeneratePdfAsync<TModel>(TModel model, CancellationToken cancellationToken = default);
}
