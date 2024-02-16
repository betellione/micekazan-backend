using QuestPDF.Infrastructure;

namespace WebApp1.Services.PdfGenerator;

public interface IPdfDocumentModel
{
    IDocument GetDocument();
}