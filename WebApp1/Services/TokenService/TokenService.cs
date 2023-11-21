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

    public async Task<string?> GetCurrentOrganizerToken(Guid userId)
    {
        var token = await _context.CreatorTokens.Where(x => x.CreatorId == userId).Select(x => x.Token).FirstOrDefaultAsync();
        return token;
    }

    public async Task<string?> GetTicketToken(string barcode)
    {
        var token = await _context.Tickets
            .Where(x => x.Barcode == barcode)
            .SelectMany(x => x.Event.Creator.Tokens)
            .Select(x => x.Token)
            .FirstOrDefaultAsync();
        return token;
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
}