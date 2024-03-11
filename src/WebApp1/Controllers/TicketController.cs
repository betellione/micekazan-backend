using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data.Stores;
using WebApp1.Models;
using WebApp1.Services.ClientService;
using WebApp1.Services.PrintService;
using WebApp1.Services.TicketService;

namespace WebApp1.Controllers;

[ApiController]
[Authorize(Roles = "Scanner")]
[Route("[controller]/[action]")]
public class TicketController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IPrintService _printService;
    private readonly ITicketService _ticketService;
    private readonly UserManager<User> _userManager;
    private readonly ITokenStore _tokenStore;

    public TicketController(ITicketService ticketService, UserManager<User> userManager, LinkGenerator linkGenerator,
        IClientService clientService, IPrintService printService, ITokenStore tokenStore)
    {
        _ticketService = ticketService;
        _userManager = userManager;
        _linkGenerator = linkGenerator;
        _clientService = clientService;
        _printService = printService;
        _tokenStore = tokenStore;
    }

    [HttpPost]
    public async Task<IActionResult> PrintTicket(string code)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        var printingToken = await _tokenStore.GetScannerPrintingToken(userId);
        if (printingToken is null) return BadRequest();

        var info = await _clientService.AddClientData(code);
        if (info is null) return BadRequest();

        var handlerUri = _linkGenerator.GetUriByAction(HttpContext, "InfoToShow", "Client", new { token = info.Token, }) ??
                         string.Empty;

        await using var pdfDocument = await _ticketService.CreateTicketPdf(userId, handlerUri, info);
        if (pdfDocument is null) return BadRequest();

        if (!await _printService.AddTicketToPrintQueue(pdfDocument, printingToken, code)) return BadRequest();
        if (!await _ticketService.SetPassTime(code)) return BadRequest();

        return Ok();
    }
}