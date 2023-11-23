namespace Micekazan.PrintDispatcher.Data.FileManager;

public interface IPdfManager
{
    public Task<string?> SaveTicketPdf(Stream file, string barcode);
    public Task<Stream?> ReadTicketPdf(string fileName);
    public bool DeleteTicketPdf(string fileName);
}