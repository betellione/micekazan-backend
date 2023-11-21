namespace WebApp1.Data.Stores;

public interface IScannerStore
{
    public Task<string?> GetScannerPrintingToken(Guid scannerId);
}