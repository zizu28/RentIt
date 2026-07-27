using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using RentIt.Shared.Abstractions.Pdf;
using RentIt.Shared.Infrastructure.Pdf.Templates;

namespace RentIt.Shared.Infrastructure.Pdf;

public static class PdfExtensions
{
    public static IServiceCollection AddSharedPdfServices(this IServiceCollection services)
    {
        // Configure QuestPDF Community License
        QuestPDF.Settings.License = LicenseType.Community;

        // Optionally, register a custom default font if you provide one.
        // For now, QuestPDF will fallback to system fonts like Arial/Helvetica.
        
        services.AddSingleton<IPdfService, QuestPdfService>();

        // Register document generators here
        services.AddTransient<IDocumentGenerator<PaymentReceiptModel>, PaymentReceiptDocumentGenerator>();

        return services;
    }
}
