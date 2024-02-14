using QuestPDF.Fluent;

namespace WebApp1.Services.PdfGenerator;

public class PdfGenerator : IPdfGenerator
{
    public Stream GenerateTicketPdf(TicketDocumentModel model)
    {
        var document = new TicketDocument(model);
        var stream = new MemoryStream();

        document.GeneratePdf(stream);
        stream.Position = 0;

        return stream;
    }
}