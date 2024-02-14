using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using ILogger = Serilog.ILogger;

namespace WebApp1.Jobs;

public abstract class ImportJobBase : BackgroundService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly TimeSpan _period;
    private protected readonly ILogger Logger = Log.ForContext<ImportJobBase>();
    private protected readonly IServiceProvider ServiceProvider;

    private protected ImportJobBase(IDbContextFactory<ApplicationDbContext> contextFactory, TimeSpan period,
        IServiceProvider serviceProvider)
    {
        _contextFactory = contextFactory;
        _period = period;
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var ids = await GetImportIds(stoppingToken);
                foreach (var id in ids) await Import(id, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            Logger.Error("Import job cancelled");
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while import");
        }
    }

    private async Task<IEnumerable<Guid>> GetImportIds(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var ids = await context.CreatorTokens.Select(x => x.CreatorId).Distinct().ToListAsync(cancellationToken);
        return ids;
    }

    private protected abstract Task Import(Guid userId, CancellationToken cancellationToken = default);
}