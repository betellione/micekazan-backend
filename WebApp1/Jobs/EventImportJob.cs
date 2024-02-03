using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Data;
using WebApp1.Services.EventService;

namespace WebApp1.Jobs;

public class EventImportJob : ImportJobBase
{
    public EventImportJob(IDbContextFactory<ApplicationDbContext> contextFactory, IOptions<JobSettings> options,
        IServiceProvider serviceProvider)
        : base(contextFactory, options.Value.ImportEventsPeriod, serviceProvider)
    {
    }

    private protected override async Task Import(Guid userId, CancellationToken cancellationToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        Logger.Information("Events import started");

        var result = await eventService.ImportEvents(userId, cancellationToken);

        if (result) Logger.Information("Event import completed successfully");
        else Logger.Error("Event import competed with errors");
    }
}