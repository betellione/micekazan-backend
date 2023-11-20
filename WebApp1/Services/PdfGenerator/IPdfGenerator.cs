using WebApp1.Models;

namespace WebApp1.Services.PdfGenerator;

public interface IPdfGenerator
{
    public Stream GenerateTicketPdf(Client client, TicketPdfTemplate template);
}