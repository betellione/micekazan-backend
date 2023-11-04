using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Services.TokenService;

public class TokenService(ApplicationDbContext context) : ITokenService
{
    public async Task<string?> GetToken(Guid userId)
    {
        var creatorToken = await context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == userId);
        return creatorToken?.Token;
    }

    public async Task<bool> SetToken(Guid userId, string token)
    {
        var creatorToken = await context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == userId);

        if (creatorToken is null)
        {
            creatorToken = new CreatorToken
            {
                CreatorId = userId,
                Token = token,
            };

            context.CreatorTokens.Add(creatorToken);
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
        context.TokenUpdates.Add(update);

        try
        {
            await context.SaveChangesAsync();
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