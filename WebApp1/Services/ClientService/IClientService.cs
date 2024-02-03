using WebApp1.Models;

namespace WebApp1.Services.ClientService;

public interface IClientService
{
    public Task<bool> ImportClients(Guid userId, CancellationToken cancellationToken = default);
    public Task<InfoToShow?> AddClientData(string ticketBarcode);
    public Task<InfoToShow?> GetClientData(string token);
}