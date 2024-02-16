using QuestPDF.Fluent;

namespace WebApp1.Services.PdfGenerator;

public class PdfGenerator : IPdfGenerator
{
    public Stream GeneratePdfDocument(IPdfDocumentModel model)
    {
        var document = model.GetDocument();
        var stream = new MemoryStream();

        document.GeneratePdf(stream);
        stream.Position = 0;

        return stream;
    }
}