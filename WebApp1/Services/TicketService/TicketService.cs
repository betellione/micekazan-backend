using WebApp1.External.Qtickets;

namespace WebApp1.Services.TicketService;

public class TicketService(IQticketsApiProvider apiProvider) : ITicketService
{
    public async Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        var ticket = await apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        return ticket?.PdfUri;
    }
}