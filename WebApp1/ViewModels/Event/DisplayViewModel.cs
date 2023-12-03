namespace WebApp1.ViewModels.Event;

public class DisplayViewModel
{
    public long EventId { get; set; }
    public ScreenViewModel WaitingDisplayViewModel { get; set; } = new();
    public ScreenViewModel SuccessDisplayViewModel { get; set; } = new();
    public ScreenViewModel FailDisplayViewModel { get; set; } = new();
}