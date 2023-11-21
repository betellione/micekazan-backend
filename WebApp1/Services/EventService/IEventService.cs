using WebApp1.Models;

namespace WebApp1.Services.EventService;

public interface IEventService
{
    public Task<bool> ImportEvents(Guid userId);
    public Task GetById();
    public Task<IEnumerable<Event>> GetAll(Guid userId);
}