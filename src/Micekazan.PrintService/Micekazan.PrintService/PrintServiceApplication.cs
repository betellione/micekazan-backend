namespace Micekazan.PrintService;

public class PrintServiceApplication
{
    private readonly WebApplication _app;

    public PrintServiceApplication(WebApplication app)
    {
        _app = app;
    }

    public void Run()
    {
        var url = _app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        PrintMessages(url);
        _app.Run(url);
    }

    private static void PrintMessages(string tokenUrl)
    {
        Console.WriteLine("Сервис печати готов и может принимать входящие запросы на печать.");
        Console.WriteLine($"Найти токен этого сервиса печати вы можете по адресу {tokenUrl}");
        Console.WriteLine("Будет использован принтер по умолчанию.");
    }
}