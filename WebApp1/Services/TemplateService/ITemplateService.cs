using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Services.TemplateService;

public interface ITemplateService
{
    Task<long> AddTemplate(Guid userId, TemplateViewModel vm);
    Task<TicketPdfTemplate?> GetTemplate(long id);
    Task<TicketPdfTemplate?> GetTemplateForScanner(Guid scannerId);
    Task<IEnumerable<long>> GetTemplateIds(Guid userId);
    Task UpdateTemplate(TemplateViewModel vm);
}