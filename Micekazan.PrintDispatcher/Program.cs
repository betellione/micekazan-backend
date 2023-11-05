using System.Collections.Concurrent;
using Micekazan.PrintDispatcher.Contracts;
using Micekazan.PrintDispatcher.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:Default"]));

var app = builder.Build();

var updateId = 0L;
var dick = new ConcurrentDictionary<long, string>();

app.MapGet("/api/update", async (ApplicationDbContext context) =>
{
    var toPrint = await context.TicketsToPrint.OrderBy(x => x.CreatedAt).FirstOrDefaultAsync();
    if (toPrint is null) return Results.NoContent();

    // const string uri = "https://qtickets.ru/ticket/pdf/xv1OsQ6rSg/ae7a8a67c9d9addaedb1ccbeb02fdb00";

    var document = new Update
    {
        Kind = UpdateKind.PrintCommand,
        Id = Interlocked.Increment(ref updateId),
        Document = new Document
        {
            DocumentId = "qwerty",
            DocumentUri = toPrint.Url,
        },
    };

    dick.TryAdd(document.Id, toPrint.Barcode);

    return Results.Ok(document);
});

app.MapPost("/api/ack", async (Acknowledgement ack, ApplicationDbContext context) =>
{
    if (dick.TryGetValue(ack.UpdateId, out var barcode)) return Results.NotFound();

    var toPrint = await context.TicketsToPrint.FindAsync(barcode);
    if (toPrint is null) return Results.NotFound();

    context.TicketsToPrint.Remove(toPrint);
    await context.SaveChangesAsync();

    return Results.Ok();
});

app.Run();