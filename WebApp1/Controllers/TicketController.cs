using Microsoft.AspNetCore.Mvc;

namespace WebApp1.Controllers;

public class TicketController : Controller
{
    public string PrintTicket(string code)
    {
        var barcodes = new TicketsToPrint();
        return code;
    }
}