using Micekazan.PrintService;
using Micekazan.PrintService.PrintProvider.Windows;

var printProvider = new WindowsPrintProvider();
var builder = new PrintServiceApplicationBuilder(args);

await builder.Configure(printProvider);

var app = builder.Build();

app.Run();