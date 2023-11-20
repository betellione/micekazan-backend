namespace WebApp1.ViewModels.Settings;

public class PrintViewModel
{
    public IEnumerable<long> TemplateIds { get; set; } = Enumerable.Empty<long>();
    public long? SelectedTemplateId { get; set; }
    public TemplateViewModel TemplateViewModel { get; set; } = new();
}