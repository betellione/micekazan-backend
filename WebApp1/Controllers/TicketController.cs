using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.TicketService;

namespace WebApp1.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ApplicationDbContext _context;

    public TicketController(ITicketService ticketService, ApplicationDbContext context)
    {
        _ticketService = ticketService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PrintTicket(string code)
    {
        var pdfUri = await _ticketService.GetTicketPdfUri(code);
        if (pdfUri is null) return BadRequest();

        var ticket = new TicketToPrint
        {
            Barcode = code,
            CreatedAt = DateTime.UtcNow,
            Url = pdfUri,
        };

        _context.TicketsToPrint.Add(ticket);
        await _context.SaveChangesAsync();

        return Ok();
    }
}