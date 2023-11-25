namespace WebApp1.ViewModels.Settings;

public class DisplayViewModel
{
    public IEnumerable<Models.Event> Events { get; set; } = Enumerable.Empty<Models.Event>();
    public long? SelectedEventId { get; set; }
    public ScreenViewModel? WaitingDisplayViewModel { get; set; }
    public ScreenViewModel? SuccessDisplayViewModel { get; set; }
    public ScreenViewModel? FailDisplayViewModel { get; set; }
}