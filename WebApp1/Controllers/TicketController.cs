using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.TicketService;

namespace WebApp1.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class TicketController(ITicketService ticketService, ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PrintTicket(string code)
    {
        var pdfUri = await ticketService.GetTicketPdfUri(code);
        if (pdfUri is null) return BadRequest();
        
        var result = await ticketService.CreateTicket(code);
        if (result is null) return Problem();

        var ticket = new TicketToPrint
        {
            Barcode = code,
            CreatedAt = DateTime.UtcNow,
            Url = pdfUri,
        };

        context.TicketsToPrint.Add(ticket);
        await context.SaveChangesAsync();

        return Ok();
    }
}