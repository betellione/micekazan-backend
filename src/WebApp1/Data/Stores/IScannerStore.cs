using WebApp1.Models;

namespace WebApp1.Data.Stores;

public interface IScannerStore
{
    Task<EventScanner?> FindScannerById(Guid userId);
    Task<bool> SetClaimsForScanner(Guid userId, bool isAutomate);
}