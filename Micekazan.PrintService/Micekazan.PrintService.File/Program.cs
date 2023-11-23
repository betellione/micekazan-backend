using Micekazan.PrintService;
using Micekazan.PrintService.PrintProvider.File;

var printProvider = new FilePrintProvider();
var printService = new PrintServiceApplication(args);

await printService.Configure(printProvider);

var app = printService.Build();

app.Run();