using System.Security.Cryptography;
using System.Text.Json;

namespace Micekazan.PrintService;

public class MicekazanConfigurationManager
{
    private const string FileName = ".micekazanConfig.json";
    private const int TokenBytes = 32;
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
        var configuration = new MicekazanConfiguration { Token = GenerateToken(TokenBytes), };
        var json = JsonSerializer.Serialize(
            configuration, MicekazanConfigurationJsonSerializerContext.Default.MicekazanConfiguration);
        await File.WriteAllTextAsync(FileName, json);

        Console.WriteLine("Не было найдено существующей конфигурации.");
        Console.WriteLine("Был сгенерирован токен сервиса.");

        return configuration;
    }

    private static async Task<MicekazanConfiguration> ExistingConfiguration()
    {
        try
        {
            var json = await File.ReadAllTextAsync(FileName);
            var configuration = JsonSerializer.Deserialize(
                json, MicekazanConfigurationJsonSerializerContext.Default.MicekazanConfiguration);
            if (configuration is null) throw new Exception();
            configuration.Validate();

            Console.WriteLine("Была найдена существующая конфигурация.");
            Console.WriteLine("Будет использован найденный токен сервиса.");

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
        Console.WriteLine($"Micekazan {DateTime.Now.Year} Сервис печати.");

        if (File.Exists(FileName)) _configuration = await ExistingConfiguration();
        else _configuration = await NewConfiguration();
    }
}