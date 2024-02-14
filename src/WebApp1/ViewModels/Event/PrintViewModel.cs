namespace WebApp1.ViewModels.Event;

public class PrintViewModel
{
    public long EventId { get; set; }
    public IEnumerable<long> TemplateIds { get; set; } = Enumerable.Empty<long>();
    public long? SelectedTemplateId { get; set; }
    public TemplateViewModel TemplateViewModel { get; set; } = new();
}