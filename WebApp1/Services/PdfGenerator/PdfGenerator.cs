using QuestPDF.Fluent;
using WebApp1.Models;

namespace WebApp1.Services.PdfGenerator;

public class PdfGenerator : IPdfGenerator
{
    public Stream GenerateTicketPdf(Client client, TicketPdfTemplate template)
    {
        var model = new TicketDocumentModel
        {
            Name = client.Name,
            Surname = client.Surname,
            IsHorizontal = template.IsHorizontal,
            FontColor = template.TextColor,
            BackgroundPath = template.BackgroundUri,
            LogoPath = template.LogoUri,
            QrPath = null,
        };

        var document = new TicketDocument(model);
        var stream = new MemoryStream();

        document.GeneratePdf(stream);

        return stream;
    }
}