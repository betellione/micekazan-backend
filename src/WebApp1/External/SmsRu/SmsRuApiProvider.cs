using System.Net;
using Microsoft.Extensions.Options;
using Serilog;
using WebApp1.External.SmsRu.Contracts.Responses;
using WebApp1.Options;
using ILogger = Serilog.ILogger;

namespace WebApp1.External.SmsRu;

public class SmsRuApiProvider : ISmsRuApiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger = Log.ForContext<ISmsRuApiProvider>();
    private readonly SmsOptions _options;

    public SmsRuApiProvider(IHttpClientFactory httpClientFactory, IOptions<SmsOptions> optionsAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _options = optionsAccessor.Value;
    }

    public async Task<bool> SendSms(string phoneNumber, string message)
    {
        var client = _httpClientFactory.CreateClient("SmsRu");
        var token = _options.Token;

        try
        {
            _logger.Information("Sending SMS '{Message}' to number {PhoneNumber}", message, phoneNumber);

            using var response = await client.PostAsync($"sms/send?api_id={token}&to={phoneNumber}&msg={message}&json=1", null);
            var json = await response.Content.ReadFromJsonAsync<SendSms>();

            _logger.Information("SMS sent with response {SendResponse}", json);

            return json?.Sms.Values.FirstOrDefault()?.Status == "OK";
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while sending SMS to a number {PhoneNumber}", phoneNumber);
            return false;
        }
    }

    public async Task<string?> MakePhoneCall(string phoneNumber, IPAddress? ip = null)
    {
        var client = _httpClientFactory.CreateClient("SmsRu");
        client.Timeout = TimeSpan.FromSeconds(300);
        var token = _options.Token;

        try
        {
            _logger.Information("Making a phone call to number {PhoneNumber}", phoneNumber);

            // TODO: Check if IP is not in private network or is not blocked or smth.
            // var ipParameter = ip is null ? string.Empty : $"&ip={ip}";
            var ipParameter = string.Empty;
            using var response = await client.PostAsync($"code/call?phone={phoneNumber}{ipParameter}&api_id={token}", null);
            var json = await response.Content.ReadFromJsonAsync<PhoneCallResponse>();

            _logger.Information("Phone call made with response {PhoneCallResponse}", json);

            return json?.Status == "OK" ? json.Code.ToString("0000") : null;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while making a phone call to a number {PhoneNumber}", phoneNumber);
            return null;
        }
    }
}