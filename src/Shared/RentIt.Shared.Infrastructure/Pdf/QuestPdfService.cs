using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using RentIt.Shared.Abstractions.Pdf;

namespace RentIt.Shared.Infrastructure.Pdf;

internal sealed class QuestPdfService(IServiceProvider serviceProvider) : IPdfService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public byte[] GeneratePdf<TModel>(TModel model)
    {
        var generator = _serviceProvider.GetRequiredService<IDocumentGenerator<TModel>>();
        var document = generator.CreateDocument(model);
        return document.GeneratePdf();
    }

    public Task<byte[]> GeneratePdfAsync<TModel>(TModel model, CancellationToken cancellationToken = default)
    {
        // QuestPDF generation is synchronous by nature, but we can wrap it in Task.Run if it's CPU bound,
        // or just return completed task. Given it's CPU bound, running it synchronously here is fine,
        // or returning Task.FromResult. 
        // We will just execute it synchronously and wrap in Task.FromResult to satisfy the interface.
        var bytes = GeneratePdf(model);
        return Task.FromResult(bytes);
    }
}
