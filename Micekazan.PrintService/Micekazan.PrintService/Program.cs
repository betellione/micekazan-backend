using System.Threading.Channels;
using Micekazan.PrintDispatcher.Contracts;
using Micekazan.PrintService;
using Micekazan.PrintService.PrintProvider;
using Micekazan.PrintService.PrintProvider.Windows;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHttpClient("PrintApi", client => { client.BaseAddress = new Uri(builder.Configuration["PrintApiUri"]!); });
builder.Services.AddHttpClient("Qtickets");

builder.Services.AddHostedService<PrintApiPooler>();

builder.Services.AddSingleton(Channel.CreateUnbounded<Document>());
builder.Services.AddHostedService<PrintQueue>();
builder.Services.AddSingleton<IPrintProvider, WindowsPrintProvider>();

var app = builder.Build();

app.Run();