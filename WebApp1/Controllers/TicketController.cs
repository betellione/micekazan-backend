using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.TicketService;

namespace WebApp1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TicketController(ITicketService ticketService, ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PrintTicket(string code)
    {
        
        // TODO: Generate PDF if doesn't exist,
        var pdfUri = await ticketService.GetTicketPdfUri(code);
        if (pdfUri is null) return BadRequest();

        var ticketToPrint = new TicketToPrint
        {
            Barcode = code,
            CreatedAt = DateTime.UtcNow,
            Url = pdfUri,
        };
        
        

        context.TicketsToPrint.Add(ticketToPrint);
        await context.SaveChangesAsync();

        return Ok();
    }
}