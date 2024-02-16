namespace WebApp1.Services.PdfGenerator;

public interface IPdfGenerator
{
    Stream GeneratePdfDocument(IPdfDocumentModel model);
}