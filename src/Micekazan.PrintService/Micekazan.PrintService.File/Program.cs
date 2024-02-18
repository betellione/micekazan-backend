using Micekazan.PrintService;
using Micekazan.PrintService.PrintProvider.File;

var printProvider = new FilePrintProvider();
var builder = new PrintServiceApplicationBuilder(args);

await builder.Configure(printProvider);

var app = builder.Build();

app.Run();