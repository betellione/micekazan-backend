namespace WebApp1.Services.PdfGenerator;

public interface IPdfGenerator
{
    public Stream GenerateTicketPdf(TicketDocumentModel model);
}