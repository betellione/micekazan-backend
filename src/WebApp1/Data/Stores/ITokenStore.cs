namespace WebApp1.Data.Stores;

public interface ITokenStore
{
    Task<string?> GetCurrentOrganizerToken(Guid userId);
    Task<string?> GetTicketToken(string barcode);
    Task<string?> GetScannerPrintingToken(Guid scannerId);
    Task<bool> SetOrganizerQticketsToken(Guid organizerId, string token);
}