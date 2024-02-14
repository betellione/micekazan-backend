namespace Micekazan.PrintDispatcher.Data.FileManager;

public interface IPdfManager
{
    Task<string?> SaveTicketPdf(Stream file, string barcode);
    Task<Stream?> ReadTicketPdf(string fileName);
    bool DeleteTicketPdf(string fileName);
}