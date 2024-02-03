using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Data;
using WebApp1.Services.ClientService;

namespace WebApp1.Jobs;

public class ClientImportJob : ImportJobBase
{
    public ClientImportJob(IDbContextFactory<ApplicationDbContext> contextFactory, IOptions<JobSettings> options,
        IServiceProvider serviceProvider)
        : base(contextFactory, options.Value.ImportClientsPeriod, serviceProvider)
    {
    }

    private protected override async Task Import(Guid userId, CancellationToken cancellationToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var clientService = scope.ServiceProvider.GetRequiredService<IClientService>();

        Logger.Information("Clients import started");

        var result = await clientService.ImportClients(userId, cancellationToken);

        if (result) Logger.Information("Clients import completed successfully");
        else Logger.Error("Clients import competed with errors");
    }
}