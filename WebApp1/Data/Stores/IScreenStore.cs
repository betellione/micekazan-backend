using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Data.Stores;

public interface IScreenStore
{
    public Task<Screen?> GetScreenByType(long eventId, ScreenTypes type);
    public Task<long> AddTemplate(long eventId, ScreenViewModel vm, ScreenTypes type);
}