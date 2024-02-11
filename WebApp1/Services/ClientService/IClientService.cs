using WebApp1.Models;

namespace WebApp1.Services.ClientService;

public interface IClientService
{
    Task<bool> ImportClients(Guid userId, CancellationToken cancellationToken = default);
    Task<InfoToShow?> AddClientData(string ticketBarcode);
    Task<InfoToShow?> GetClientData(string token);
}