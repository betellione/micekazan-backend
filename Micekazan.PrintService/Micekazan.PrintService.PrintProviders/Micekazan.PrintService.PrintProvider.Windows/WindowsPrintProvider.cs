namespace Micekazan.PrintService.PrintProvider.Windows;

public class WindowsPrintProvider : IPrintProvider
{
    public Task PrintDocument(Stream document, PrintSettings? settings)
    {
        // throw new NotImplementedException();
        using var pdfDocument = new PdfDocument(document);
        pdfDocument.Print();
        return Task.CompletedTask;
    }
}