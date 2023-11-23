namespace Micekazan.PrintDispatcher.Data.FileManager;

public class PdfManager : IPdfManager
{
    private readonly IFileManager _fileManager;

    public PdfManager(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public Task<string?> SaveTicketPdf(Stream file, string barcode)
    {
        var fileName = $"{Guid.NewGuid():N}_{barcode}.pdf";
        var path = _fileManager.SaveFile(file, fileName);
        return path;
    }

    public Task<Stream?> ReadTicketPdf(string fileName)
    {
        var file = _fileManager.ReadFile(fileName);
        return Task.FromResult(file);
    }

    public bool DeleteTicketPdf(string fileName)
    {
        return _fileManager.DeleteFile(fileName);
    }
}