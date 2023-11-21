using Microsoft.EntityFrameworkCore;

namespace WebApp1.Data.Stores;

public class ScannerStore : IScannerStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ScannerStore(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string?> GetScannerPrintingToken(Guid scannerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.EventScanners.Where(x => x.ScannerId == scannerId).Select(x => x.PrintingToken).FirstOrDefaultAsync();
    }
}