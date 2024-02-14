using WebApp1.External.SmsRu;

namespace WebApp1.Services.SmsSender;

public class SmsSender : ISmsSender
{
    private readonly ISmsRuApiProvider _api;

    public SmsSender(ISmsRuApiProvider api)
    {
        _api = api;
    }

    public async Task SendSmsAsync(string number, string message)
    {
        var result = await _api.SendSms(number, message);
        if (!result) throw new Exception("SMS was not sent");
    }
}