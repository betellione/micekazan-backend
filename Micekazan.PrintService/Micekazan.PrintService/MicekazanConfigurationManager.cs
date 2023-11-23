using System.Security.Cryptography;
using System.Text.Json;

namespace Micekazan.PrintService;

public class MicekazanConfigurationManager
{
    private const string FileName = ".micekazanConfig.json";
    private const int TokenBytes = 32;
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { WriteIndented = true, };
    private MicekazanConfiguration? _configuration;

    public MicekazanConfiguration Configuration => _configuration ?? throw new Exception("App has not been configured.");

    private static string GenerateToken(int bytes)
    {
        // 24 bytes gives 32 char length string in base64 without trailing '=' and 192 bytes gives 256 char length string.
        if (bytes is < 24 or > 192)
        {
            throw new InvalidOperationException($"Parameter {nameof(bytes)} must be greater or equal to {24} or less or equal to {192}");
        }

        var tokenData = new byte[bytes];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenData);
        var token = Convert.ToBase64String(tokenData).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return token;
    }

    private static async Task<MicekazanConfiguration> NewConfiguration()
    {
        var config = new MicekazanConfiguration { Token = GenerateToken(TokenBytes), };
        var json = JsonSerializer.Serialize(config, Options);
        await File.WriteAllTextAsync(FileName, json);

        PrintNewConfigurationMessage();

        return config;
    }

    private static async Task<MicekazanConfiguration> ExistingConfiguration()
    {
        try
        {
            var json = await File.ReadAllTextAsync(FileName);
            var configuration = JsonSerializer.Deserialize<MicekazanConfiguration>(json, Options);
            if (configuration is null) throw new Exception();
            configuration.Validate();
            return configuration;
        }
        catch
        {
            Console.WriteLine("Ошибка: файл конфигурации имеет неправильный формат.");
            Console.WriteLine($"Попробуйте удалить файл {FileName} и перезапустить приложение.");
            Console.WriteLine("Выход. Нажмите любую клавишу...");
            Console.ReadKey();
            Environment.Exit(1);
            throw;
        }
    }

    public async Task Configure()
    {
        PrintWelcomeMessage();

        if (!File.Exists(FileName))
        {
            _configuration = await NewConfiguration();
            return;
        }

        _configuration = await ExistingConfiguration();

        PrintConfigurationMessage();
        PrintReadyMessage();
    }

    #region PrintingMessages

    private static void PrintWelcomeMessage()
    {
        Console.WriteLine($"Micekazan {DateTime.Now.Year} Сервис печати.");
    }

    private static void PrintNewConfigurationMessage()
    {
        Console.WriteLine("Не было найдено существующей конфигурации.");
        Console.WriteLine("Токен сервиса был сгенерирован заново.");
    }

    private static void PrintConfigurationMessage()
    {
        Console.WriteLine("Найти токен этого сервиса печати вы можете по адресу http://localhost:5038");
    }

    private static void PrintReadyMessage()
    {
        Console.WriteLine("Сервис печати готов и может принимать входящие запросы на печать.");
        Console.WriteLine("Будет исполььзован принтер по умолчанию.");
    }

    #endregion
}