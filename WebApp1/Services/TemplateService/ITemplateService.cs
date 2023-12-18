using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Services.TemplateService;

public interface ITemplateService
{
    public Task<long> AddTemplate(Guid userId, TemplateViewModel vm);
    public Task<TicketPdfTemplate?> GetTemplate(long id);
    public Task<TicketPdfTemplate?> GetTemplateForScanner(Guid scannerId);
    public Task<IEnumerable<long>> GetTemplateIds(Guid userId);
    public Task UpdateTemplate(TemplateViewModel vm);
}