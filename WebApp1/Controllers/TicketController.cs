using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data.Stores;
using WebApp1.Models;
using WebApp1.Services.PrintService;
using WebApp1.Services.TicketService;

namespace WebApp1.Controllers;

[Authorize(Roles = "Scanner")]
[ApiController]
[Route("[controller]/[action]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly UserManager<User> _userManager;
    private readonly IPrintService _printService;
    private readonly IScannerStore _scannerStore;

    public TicketController(ITicketService ticketService, UserManager<User> userManager, IPrintService printService,
        IScannerStore scannerStore)
    {
        _ticketService = ticketService;
        _userManager = userManager;
        _printService = printService;
        _scannerStore = scannerStore;
    }

    [HttpPost]
    public async Task<IActionResult> PrintTicket(string code)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        var printingToken = await _scannerStore.GetScannerPrintingToken(userId);
        if (printingToken is null) return BadRequest();

        await using var pdf = await _ticketService.GetTicketPdf(userId, code);
        if (pdf is null) return BadRequest();

        await _printService.AddTicketToPrintQueue(pdf, printingToken);

        return Ok();
    }
}