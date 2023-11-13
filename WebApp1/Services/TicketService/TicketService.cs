namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    public Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        // var ticket = await apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        return Task.FromResult((string?)string.Empty);
    }
}