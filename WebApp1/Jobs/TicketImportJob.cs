using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Data;
using WebApp1.Services.TicketService;

namespace WebApp1.Jobs;

public class TicketImportJob : ImportJobBase
{
    public TicketImportJob(IDbContextFactory<ApplicationDbContext> contextFactory, IOptions<JobSettings> options,
        IServiceProvider serviceProvider)
        : base(contextFactory, options.Value.ImportTicketsPeriod, serviceProvider)
    {
    }

    private protected override async Task Import(Guid userId, CancellationToken cancellationToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();

        Logger.Information("Tickets import started");

        var result = await ticketService.ImportTickets(userId, cancellationToken);

        if (result) Logger.Information("Tickets import completed successfully");
        else Logger.Error("Tickets import competed with errors");
    }
}