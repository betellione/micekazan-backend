using WebApp1.External.SmsRu;

namespace WebApp1.Services.SmsSender;

public class SmsSender(ISmsRuApiProvider api) : ISmsSender
{
    public async Task SendSmsAsync(string number, string message)
    {
        var result = await api.SendSms(number, message);
        if (!result) throw new Exception("SMS was not sent");
    }
}