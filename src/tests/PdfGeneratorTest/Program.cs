using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using WebApp1.Services.PdfGenerator;

Settings.License = LicenseType.Community;
var fontStream = File.OpenRead("Montserrat-Bold.ttf");
FontManager.RegisterFont(fontStream);

var qrStream = File.OpenRead("qr.png");
var model = new TicketDocumentModel
{
    Name = "Сергей",
    Surname = "Прокофьев",
    BackgroundPath = null,
    FontColor = "#000000",
    IsHorizontal = true,
    LogoPath = "logo.jpg",
    QrStream = qrStream,
};

var pdfGenerator = new PdfGenerator();

var horizontal = pdfGenerator.GenerateTicketPdf(model);
var horizontalFile = File.OpenWrite("horizontal.pdf");
horizontal.CopyTo(horizontalFile);

model.IsHorizontal = false;
qrStream.Position = 0;

var vertical = pdfGenerator.GenerateTicketPdf(model);
var verticalFile = File.OpenWrite("vertical.pdf");
vertical.CopyTo(verticalFile);