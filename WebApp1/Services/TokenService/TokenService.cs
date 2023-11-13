using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _context;

    public TokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetToken(Guid userId)
    {
        var creatorToken = await _context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == userId);
        return creatorToken?.Token;
    }

    public async Task<bool> SetToken(Guid userId, string token)
    {
        var creatorToken = await _context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == userId);

        if (creatorToken is null)
        {
            creatorToken = new CreatorToken
            {
                CreatorId = userId,
                Token = token,
            };

            _context.CreatorTokens.Add(creatorToken);
        }
        else
        {
            creatorToken.Token = token;
        }

        var update = new TokenUpdate
        {
            CreatorId = userId,
            Token = token,
            UpdatedAt = DateTime.UtcNow,
        };
        _context.TokenUpdates.Add(update);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    public Task<bool> RemoveToken(Guid userId)
    {
        throw new NotImplementedException();
    }
}