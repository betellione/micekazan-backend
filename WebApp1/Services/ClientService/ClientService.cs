using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.ClientService;

public class ClientService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
    IQticketsApiProvider apiProvider) : IClientService
{
    private readonly ILogger _logger = Log.ForContext<IClientService>();

    public async Task<bool> ImportClients(Guid userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        
        var token = await tokenService.GetToken(userId);
        if (token is null) return false;

        var clients = apiProvider.GetClients(token).Select(x => new Client
        {
            Email = x.Email,
            Name = x.Name,
            Patronymic = x.Patronymic,
            Surname = x.Surname,
            ForeignId = x.Id,
            PhoneNumber = x.PhoneNumber,
        });

        var batchCounter = 0;
        var existed = (await context.Clients.Select(x => x.Email).ToListAsync()).ToHashSet();

        await foreach (var client in clients)
        {
            if (existed.Contains(client.Email)) context.Update(client);
            else context.Clients.Add(client);

            try
            {
                if (++batchCounter >= 100)
                {
                    batchCounter = 0;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving clients in the DB");
            }
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while saving clients in the DB");
        }

        return true;
    }
}