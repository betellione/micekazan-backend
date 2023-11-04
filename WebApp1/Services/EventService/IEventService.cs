namespace WebApp1.Services.EventService;

public interface IEventService
{
    public Task ImportEvents();
    public Task GetById();
}