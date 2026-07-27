using QuestPDF.Infrastructure;

namespace RentIt.Shared.Infrastructure.Pdf;

public interface IDocumentGenerator<in TModel>
{
    IDocument CreateDocument(TModel model);
}
