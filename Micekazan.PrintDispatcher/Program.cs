using Micekazan.PrintDispatcher.Contracts;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

long id = 1;

app.MapGet("/api/update", () =>
{
    if (id > 1) return Results.NoContent();

    const string uri = "https://qtickets.ru/ticket/pdf/xv1OsQ6rSg/ae7a8a67c9d9addaedb1ccbeb02fdb00";

    var document = new Update
    {
        Kind = UpdateKind.PrintCommand,
        Id = id,
        Document = new Document
        {
            DocumentId = "qwerty",
            DocumentUri = uri,
        },
    };

    return Results.Ok(document);
});

app.MapPost("/api/ack", (Acknowledgement ack) =>
{
    id += 1;
    return Results.Ok();
});

app.Run();