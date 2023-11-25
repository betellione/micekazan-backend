using WebApp1.Enums;
using WebApp1.Models;

namespace WebApp1.Data.Stores;

public interface IScreenStore
{
    public Task<Screen?> GetScreenByType(long eventId, ScreenTypes type);
}