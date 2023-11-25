using System.Net;

namespace WebApp1.External.SmsRu;

public interface ISmsRuApiProvider
{
    public Task<bool> SendSms(string phoneNumber, string message);
    public Task<string?> MakePhoneCall(string phoneNumber, IPAddress? ip = null);
}