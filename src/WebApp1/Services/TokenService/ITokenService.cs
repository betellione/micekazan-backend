namespace WebApp1.Services.TokenService;

public interface ITokenService
{
    Task<string?> GetCurrentOrganizerToken(Guid userId);
    Task<string?> GetTicketToken(string barcode);
    Task<bool> SetToken(Guid userId, string token);
}