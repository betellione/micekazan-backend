using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.Data.Stores;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.ClientService;

public class ClientService : IClientService
{
    private readonly IQticketsApiProvider _apiProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<IClientService>();
    private readonly ITokenStore _tokenStore;

    public ClientService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenStore tokenStore,
        IQticketsApiProvider apiProvider)
    {
        _contextFactory = contextFactory;
        _tokenStore = tokenStore;
        _apiProvider = apiProvider;
    }

    public async Task<bool> ImportClients(Guid userId, CancellationToken cancellationToken = default)
    {
        const int batchSize = 100;

        var token = await _tokenStore.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var batchWorker = new ClientBatchWorker(_contextFactory, batchSize);
        await using var newClientsEnumerator = _apiProvider
            .GetClients(token, cancellationToken)
            .Select(x => new Client
            {
                Email = x.Email,
                Name = x.Name,
                Patronymic = x.Patronymic,
                Surname = x.Surname,
                ForeignId = x.Id,
                PhoneNumber = x.PhoneNumber,
            })
            .GetAsyncEnumerator(cancellationToken);
        await using var oldClientsEnumerator = new ClientForeignIdsBatchRetriever(_contextFactory, batchSize)
            .Retrieve(cancellationToken)
            .GetAsyncEnumerator(cancellationToken);

        if (!await newClientsEnumerator.MoveNextAsync()) return true;
        if (!await oldClientsEnumerator.MoveNextAsync())
        {
            await SaveRemainingClients(batchWorker, newClientsEnumerator, cancellationToken);
            return true;
        }

        while (true)
        {
            var oldForeignId = oldClientsEnumerator.Current;
            var newClient = newClientsEnumerator.Current;

            if (oldForeignId == newClient.ForeignId)
            {
                await batchWorker.UpdateClient(newClient, cancellationToken);
                if (!await newClientsEnumerator.MoveNextAsync()) break;
                if (!await oldClientsEnumerator.MoveNextAsync())
                {
                    await SaveRemainingClients(batchWorker, newClientsEnumerator, cancellationToken);
                    return true;
                }
            }
            else if (oldForeignId > newClient.ForeignId)
            {
                await batchWorker.AddClient(newClient, cancellationToken);
                if (!await newClientsEnumerator.MoveNextAsync()) break;
            }
            else
            {
                if (!await oldClientsEnumerator.MoveNextAsync())
                {
                    await batchWorker.AddClient(newClient, cancellationToken);
                    await SaveRemainingClients(batchWorker, newClientsEnumerator, cancellationToken);
                    return true;
                }
            }
        }

        await batchWorker.SaveRemained(cancellationToken);
        return true;

        static async Task SaveRemainingClients(ClientBatchWorker worker, IAsyncEnumerator<Client> enumerator,
            CancellationToken cancellationToken = default)
        {
            while (await enumerator.MoveNextAsync()) await worker.AddClient(enumerator.Current, cancellationToken);
            await worker.SaveRemained(cancellationToken);
        }
    }

    public async Task<InfoToShow?> AddClientData(string ticketBarcode)
    {
        var token = await _tokenStore.GetTicketToken(ticketBarcode);
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

    public async Task<InfoToShow?> GetClientData(string token)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.InfoToShow.FirstOrDefaultAsync(x => x.Token == token);
    }
}

file class ClientBatchWorker
{
    private readonly int _batchSize;
    private readonly List<Client> _clientsToAdd;
    private readonly Dictionary<long, Client> _clientsToUpdate;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<ClientBatchWorker>();

    public ClientBatchWorker(IDbContextFactory<ApplicationDbContext> contextFactory, int batchSize)
    {
        _contextFactory = contextFactory;
        _batchSize = batchSize;
        _clientsToAdd = new List<Client>(batchSize);
        _clientsToUpdate = new Dictionary<long, Client>(batchSize);
    }

    public async ValueTask UpdateClient(Client newClientData, CancellationToken cancellationToken = default)
    {
        _clientsToUpdate.TryAdd(newClientData.ForeignId, newClientData);
        if (_clientsToUpdate.Count != _batchSize) return;
        await UpdateClientSave(cancellationToken);
    }

    public async ValueTask AddClient(Client client, CancellationToken cancellationToken = default)
    {
        _clientsToAdd.Add(client);
        if (_clientsToAdd.Count != _batchSize) return;
        await AddClientSave(cancellationToken);
    }

    public async Task SaveRemained(CancellationToken cancellationToken = default)
    {
        await AddClientSave(cancellationToken);
        await UpdateClientSave(cancellationToken);
    }

    private async Task UpdateClientSave(CancellationToken cancellationToken = default)
    {
        if (_clientsToUpdate.Count == 0) return;

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var foreignIds = _clientsToUpdate.Keys.ToArray();
        var oldClients = await context.Clients
            .Where(x => foreignIds.Contains(x.ForeignId))
            .ToDictionaryAsync(x => x.ForeignId, x => x, cancellationToken);

        foreach (var (foreignId, newData) in _clientsToUpdate)
        {
            if (oldClients.TryGetValue(foreignId, out var oldData)) UpdateClientData(oldData, newData);
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while updating clients in the DB");
        }

        _clientsToUpdate.Clear();
    }

    private async Task AddClientSave(CancellationToken cancellationToken = default)
    {
        if (_clientsToAdd.Count == 0) return;

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            context.Clients.AddRange(_clientsToAdd);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while saving clients in the DB");
        }

        _clientsToAdd.Clear();
    }

    private static void UpdateClientData(Client oldData, Client newData)
    {
        oldData.Name = newData.Name;
        oldData.Surname = newData.Surname;
        oldData.Patronymic = newData.Patronymic;
        oldData.Email = newData.Email;
        oldData.PhoneNumber = newData.PhoneNumber;
    }
}

file class ClientForeignIdsBatchRetriever
{
    private readonly int _batchSize;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ClientForeignIdsBatchRetriever(IDbContextFactory<ApplicationDbContext> contextFactory, int batchSize)
    {
        _contextFactory = contextFactory;
        _batchSize = batchSize;
    }

    public async IAsyncEnumerable<long> Retrieve([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lastMaxForeignId = 0L;

        while (true)
        {
            var max = lastMaxForeignId;
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var ids = await context.Clients
                .Where(x => x.ForeignId > max)
                .OrderBy(x => x.ForeignId)
                .Select(x => x.ForeignId)
                .Take(_batchSize)
                .ToListAsync(cancellationToken);

            if (ids.Count == 0) yield break;
            lastMaxForeignId = ids[^1];

            foreach (var id in ids) yield return id;
        }
    }
}