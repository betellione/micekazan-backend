using System.Threading.Channels;
using Micekazan.PrintDispatcher.Domain.Contracts;
using Micekazan.PrintService.PrintProvider;

namespace Micekazan.PrintService;

public class PrintServiceApplicationBuilder
{
    // lang=html
    private const string TokenHtml = """
                                     <html lang="en">
                                     <head>
                                         <meta charset="utf-8">
                                         <title>Micekazan Print Service</title>
                                     </head>
                                     <body>
                                     <main>
                                         <h1>Токен для организатора:</h1>
                                         <code>{0}</code>
                                     </main>
                                     </body>
                                     </html>
                                     """;

    private readonly WebApplicationBuilder _builder;

    public PrintServiceApplicationBuilder(string[] args)
    {
        _builder = WebApplication.CreateSlimBuilder(args);
        _builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, GlobalJsonSerializerContext.Default);
        });
    }

    public async Task Configure(IPrintProvider printProvider)
    {
        var configuration = new MicekazanConfigurationManager();
        await configuration.Configure();

        _builder.Services.AddHttpClient("PrintApi", client =>
        {
            client.BaseAddress = new Uri(_builder.Configuration["PrintApiUri"]!);
            client.DefaultRequestHeaders.Add("D-PrintingToken", configuration.Configuration.Token);
        });

        _builder.Services.AddHostedService<PrintApiPooler>();
        _builder.Services.AddHostedService<PrintQueue>();
        _builder.Services.AddSingleton(printProvider);
        _builder.Services.AddSingleton(configuration);
        _builder.Services.AddSingleton(Channel.CreateUnbounded<Document>());
    }

    public PrintServiceApplication Build()
    {
        var app = _builder.Build();

        app.MapGet("/", (MicekazanConfigurationManager configurationManager) =>
        {
            var html = string.Format(TokenHtml, configurationManager.Configuration.Token);
            return Results.Text(html, "text/html");
        });

        return new PrintServiceApplication(app);
    }
}