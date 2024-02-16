using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.Data.Stores;

public class TokenStore : ITokenStore
{
    private readonly ApplicationDbContext _context;

    public TokenStore(ApplicationDbContext context)
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

    public async Task<string?> GetScannerPrintingToken(Guid scannerId)
    {
        return await _context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .Select(x => x.PrintingToken)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SetOrganizerQticketsToken(Guid organizerId, string token)
    {
        var creatorToken = await _context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == organizerId);

        if (creatorToken is null)
        {
            creatorToken = new CreatorToken
            {
                CreatorId = organizerId,
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
            CreatorId = organizerId,
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