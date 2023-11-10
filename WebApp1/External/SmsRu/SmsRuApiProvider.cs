using Microsoft.Extensions.Options;
using WebApp1.External.SmsRu.Contracts.Responses;
using WebApp1.Options;
using ILogger = Serilog.ILogger;

namespace WebApp1.External.SmsRu;

public class SmsRuApiProvider(IHttpClientFactory httpClientFactory, IOptions<SmsOptions> optionsAccessor) : ISmsRuApiProvider
{
    private readonly SmsOptions _options = optionsAccessor.Value;
    private readonly ILogger _logger = Serilog.Log.ForContext<ISmsRuApiProvider>();

    public async Task<bool> SendSms(string phoneNumber, string message)
    {
        var client = httpClientFactory.CreateClient();
        var token = _options.Token;

        try
        {
            _logger.Information("Sending SMS '{Message}' to number {PhoneNumber}", message, phoneNumber);

            using var response = await client.PostAsync($"sms/send?api_id={token}&to={phoneNumber}&msg={message}&json=1", null);
            var json = await response.Content.ReadFromJsonAsync<SendSms>();

            _logger.Information("SMS sent with response {SendResponse}", json);

            return json?.Sms[phoneNumber].Status == "OK";
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while sending SMS");
            return false;
        }
    }
}