namespace WebApp1.Services.PrintService;

public class PrintServiceFileWriterMock : IPrintService
{
    public async Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken, string barcode)
    {
        try
        {
            var fileName = $"{Guid.NewGuid()}_{barcode}.pdf";
            await using var file = File.Create(fileName);
            await ticket.CopyToAsync(file);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task<bool> PrintTicket(string barcode, Guid userId)
    {
        throw new NotImplementedException();
    }
}