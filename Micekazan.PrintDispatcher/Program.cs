using System.Net.Mime;
using Micekazan.PrintDispatcher;
using Micekazan.PrintDispatcher.Data;
using Micekazan.PrintDispatcher.Data.FileManager;
using Micekazan.PrintDispatcher.Domain.Contracts;
using Micekazan.PrintDispatcher.Extensions;
using Micekazan.PrintDispatcher.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    WebRootPath = "wwwroot",
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddSingleton<PrinterQueuesManager>();
builder.AddFileManagers();

var app = builder.Build();

app.MapGet("/api/update", async ([FromHeader(Name = "D-PrintingToken")] string printingToken, PrinterQueuesManager queues) =>
{
    var queue = queues[printingToken];

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var ct = cts.Token;

    try
    {
        var document = await queue.Reader.ReadAsync(ct);
        return Results.Ok(new Update { Kind = UpdateKind.PrintCommand, Document = document, });
    }
    catch (OperationCanceledException)
    {
        return Results.Ok(new Update { Kind = UpdateKind.NoContent, });
    }
});

app.MapGet("/pdf/{documentName}", async (string documentName, IPdfManager pdfManager) =>
{
    var document = await pdfManager.ReadTicketPdf(documentName);
    return document is null ? Results.NotFound() : Results.File(document, MediaTypeNames.Application.Pdf, "ticket.pdf");
});

app.MapPost("/api/ack", () => Results.Ok());

app.MapPost("/api/enqueue", async ([FromForm] IFormFile file, [FromForm] string printingToken, [FromForm] string barcode,
    IPdfManager pdfManager, ApplicationDbContext dbContext, PrinterQueuesManager queues, HttpRequest request) =>
{
    await using var stream = file.OpenReadStream();
    var path = await pdfManager.SaveTicketPdf(stream, barcode);
    if (path is null) return Results.BadRequest();

    var ticketToPrint = new TicketToPrint
    {
        Barcode = barcode,
        FilePath = path,
        PrintingToken = printingToken,
    };

    try
    {
        dbContext.TicketsToPrint.Add(ticketToPrint);
        await dbContext.SaveChangesAsync();
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }

    var document = new Document
    {
        Id = ticketToPrint.Id,
        DocumentPath = ticketToPrint.FilePath,
        DocumentUri = $"{request.Scheme}://{request.Host}/pdf/{Path.GetFileName(path)}",
    };

    var queue = queues[printingToken];
    await queue.Writer.WriteAsync(document);

    return Results.Ok();
}).DisableAntiforgery();

await app.SetupDatabaseAsync();

app.Run();