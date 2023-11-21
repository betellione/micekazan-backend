using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.ClientService;

public class ClientService : IClientService
{
    private readonly ILogger _logger = Log.ForContext<IClientService>();
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IQticketsApiProvider _apiProvider;

    public ClientService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
        IQticketsApiProvider apiProvider)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _apiProvider = apiProvider;
    }

    public async Task<bool> ImportClients(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var token = await _tokenService.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var clients = _apiProvider.GetClients(token).Select(x => new Client
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
            if (existed.Contains(client.Email))
            {
                context.Update(client);
            }
            else
            {
                context.Clients.Add(client);
            }

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

    public async Task<InfoToShow?> AddClientData(string ticketBarcode)
    {
        var token = await _tokenService.GetTicketToken(ticketBarcode);
        if (token is null) return null;

        var data = await _apiProvider.GetTicketClientData(ticketBarcode, token);
        if (data is null) return null;

        var tokenSpan = new byte[16];
        Random.Shared.NextBytes(tokenSpan);
        var infoToken = Convert.ToBase64String(tokenSpan).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        
        var info = new InfoToShow
        {
            Email = data.ClientEmail,
            Phone = data.ClientPhone,
            ClientName = data.ClientName,
            ClientSurname = data.ClientSurname,
            OrganizationName = data.OrganizationName,
            WorkPosition = data.WorkPosition,
            ClientMiddleName = data.ClientMiddlename,
            Barcode = ticketBarcode,
            Token = infoToken,
        };

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.InfoToShow.Add(info);
            await context.SaveChangesAsync();
            return info;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot add Info to Show for ticket barcode {Barcode} in DB with data {ClientData}", ticketBarcode, data);
            return null;
        }
    }

    public Task<InfoToShow?> GetClientData(string token)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.InfoToShow.FirstOrDefaultAsync(x => x.Token == token);
    }
}