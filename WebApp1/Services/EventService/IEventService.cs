using WebApp1.Models;

namespace WebApp1.Services.EventService;

public interface IEventService
{
    Task<bool> ImportEvents(Guid userId, CancellationToken cancellationToken = default);
    Task<Event?> GetById(long id);
    Task<IEnumerable<Event>> GetAll(Guid userId);
    Task<int> GetAllTicketsNumber(Guid scannerId);
    Task<int> GetScannedTicketsNumber(Guid scannerId);
    Task<int> GetAllTicketsNumber(long eventId);
    Task<int> GetScannedTicketsNumber(long eventId);
}