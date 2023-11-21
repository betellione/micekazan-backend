using Microsoft.EntityFrameworkCore;

namespace WebApp1.Data.Stores;

public class ScannerStore : IScannerStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ScannerStore(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task<string?> GetScannerPrintingToken(Guid scannerId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.EventScanners.Where(x => x.ScannerId == scannerId).Select(x => x.PrintingToken).FirstOrDefaultAsync();
    }
}