using WebApp1.Models;

namespace WebApp1.Services.EventService;

public interface IEventService
{
    public Task<bool> ImportEvents(Guid userId, CancellationToken cancellationToken = default);
    public Task<Event?> GetById(long id);
    public Task<IEnumerable<Event>> GetAll(Guid userId);
    public Task<int> GetAllTicketsNumber(Guid scannerId);
    public Task<int> GetScannedTicketsNumber(Guid scannerId);
    public Task<int> GetAllTicketsNumber(long eventId);
    public Task<int> GetScannedTicketsNumber(long eventId);
}