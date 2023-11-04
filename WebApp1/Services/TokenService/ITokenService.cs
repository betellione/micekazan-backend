namespace WebApp1.Services.TokenService;

public interface ITokenService
{
    public Task<string?> GetToken(Guid userId);
    public Task<bool> SetToken(Guid userId, string token);
    public Task<bool> RemoveToken(Guid userId);
}