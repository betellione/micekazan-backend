using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.ClientService;

public class ClientService(ApplicationDbContext context, ITokenService tokenService, IQticketsApiProvider apiProvider) : IClientService
{
    private readonly ILogger _logger = Log.ForContext<IClientService>();

    public async Task<bool> ImportClients(Guid userId)
    {
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

        await foreach (var client in clients)
        {
            try
            {
                context.Clients.Add(client);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving client {Client} in the DB", client);
            }
        }

        return true;
    }
}