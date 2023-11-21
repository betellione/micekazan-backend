namespace WebApp1.Services.PrintService;

public class PrintServiceFileWriterMock : IPrintService
{
    public async Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken)
    {
        try
        {
            var fileName = Guid.NewGuid() + ".pdf";
            await using var file = File.Create(fileName);
            await ticket.CopyToAsync(file);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}