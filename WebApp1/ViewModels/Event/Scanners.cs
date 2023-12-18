namespace WebApp1.ViewModels.Event;

public class Scanners
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public string EventName { get; set; } = null!;
    
    public IEnumerable<Scanner> ScannersList { get; set; } = Enumerable.Empty<Scanner>();
}