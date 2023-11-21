namespace WebApp1.Services.TokenService;

public interface ITokenService
{
    public Task<string?> GetCurrentOrganizerToken(Guid userId);
    public Task<string?> GetTicketToken(string barcode);
    public Task<bool> SetToken(Guid userId, string token);
}