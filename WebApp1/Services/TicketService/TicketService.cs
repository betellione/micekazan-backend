using WebApp1.External.Qtickets;

namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    private readonly IQticketsApiProvider _apiProvider;

    public TicketService(IQticketsApiProvider apiProvider)
    {
        _apiProvider = apiProvider;
    }

    public async Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        var ticket = await _apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        return ticket?.PdfUri;
    }
}