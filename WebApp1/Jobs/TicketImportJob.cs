namespace WebApp1.Jobs;

public class TicketImportJob : BackgroundService
{
    public TicketImportJob()
    {
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}