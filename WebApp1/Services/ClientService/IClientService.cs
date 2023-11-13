namespace WebApp1.Services.ClientService;

public interface IClientService
{
    public Task<bool> ImportClients(Guid userId);
}