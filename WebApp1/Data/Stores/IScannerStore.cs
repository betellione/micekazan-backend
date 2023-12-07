using WebApp1.Models;

namespace WebApp1.Data.Stores;

public interface IScannerStore
{
    public Task<string?> GetScannerPrintingToken(Guid scannerId);
    public Task<EventScanner?> FindScannerById(Guid userId);
    public Task<bool> SetClaimsForScanner(Guid userId, bool isAutomate);
}