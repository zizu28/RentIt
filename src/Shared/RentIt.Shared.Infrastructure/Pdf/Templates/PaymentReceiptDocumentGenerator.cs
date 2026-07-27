using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RentIt.Shared.Abstractions.Pdf;

namespace RentIt.Shared.Infrastructure.Pdf.Templates;

internal sealed class PaymentReceiptDocumentGenerator : IDocumentGenerator<PaymentReceiptModel>
{
    public IDocument CreateDocument(PaymentReceiptModel model)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                
                // Using a universally available web-safe fallback since Inter might not be installed on the system
                // If Inter is downloaded later, this can simply be changed to "Inter"
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(x => ComposeHeader(x, model));
                page.Content().Element(x => ComposeContent(x, model));
                page.Footer().Element(x => ComposeFooter(x));
            });
        });
    }

    private static void ComposeHeader(IContainer container, PaymentReceiptModel model)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("PAYMENT RECEIPT").Style(titleStyle);
                column.Item().Text(text =>
                {
                    text.Span("Issue date: ").SemiBold();
                    text.Span($"{model.IssueDate:d}");
                });
                column.Item().Text(text =>
                {
                    text.Span("Receipt #: ").SemiBold();
                    text.Span(model.ReceiptNumber);
                });
            });

            row.ConstantItem(100).Height(50).Placeholder(); // Placeholder for logo
        });
    }

    private static void ComposeContent(IContainer container, PaymentReceiptModel model)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(20);

            column.Item().Row(row =>
            {
                row.RelativeItem().Component(new AddressComponent("Billed To", model.CustomerName, model.CustomerAddress, model.CustomerEmail));
                row.ConstantItem(50);
                row.RelativeItem().Component(new AddressComponent("Payment Details", $"Method: {model.PaymentMethod}", $"Transaction: {model.TransactionId}", "Status: PAID"));
            });

            column.Item().Element(x => ComposeTable(x, model));

            column.Item().PaddingRight(5).AlignRight().Text($"Total: {model.TotalAmount:C}").FontSize(14).SemiBold();
        });
    }

    private static void ComposeTable(IContainer container, PaymentReceiptModel model)
    {
        container.Table(table =>
        {
            // step 1
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            // step 2
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Description");
                header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            // step 3
            foreach (var item in model.Items.Select((value, index) => new { value, index }))
            {
                table.Cell().Element(CellStyle).Text($"{item.index + 1}");
                table.Cell().Element(CellStyle).Text(item.value.Description);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.value.UnitPrice:C}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.value.Quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.value.Total:C}");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
            x.Span(" of ");
            x.TotalPages();
        });
    }
}

internal class AddressComponent(string title, string line1, string line2, string line3) : IComponent
{
    private string Title { get; } = title;
    private string Line1 { get; } = line1;
    private string Line2 { get; } = line2;
    private string Line3 { get; } = line3;

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(5).Text(Title).SemiBold();
            column.Item().Text(Line1);
            column.Item().Text(Line2);
            column.Item().Text(Line3);
        });
    }
}
