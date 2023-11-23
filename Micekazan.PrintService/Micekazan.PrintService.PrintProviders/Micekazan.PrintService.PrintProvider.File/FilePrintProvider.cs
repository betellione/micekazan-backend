namespace Micekazan.PrintService.PrintProvider.File;

public class FilePrintProvider : IPrintProvider
{
    public async Task PrintDocument(Stream document, PrintSettings? settings)
    {
        var fileName = $"{Guid.NewGuid():N}.pdf";
        await using var file = System.IO.File.OpenWrite(fileName);
        await document.CopyToAsync(file);
    }
}