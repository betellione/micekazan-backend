using Micekazan.PrintService;
using Micekazan.PrintService.PrintProvider.Windows;

var printProvider = new WindowsPrintProvider();
var printService = new PrintServiceApplication(args);

await printService.Configure(printProvider);

var app = printService.Build();

app.Run();