namespace WebApp1.Jobs;

public class EventImportJob : BackgroundService
{
    public EventImportJob()
    {
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}