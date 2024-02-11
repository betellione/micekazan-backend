using System.Net;

namespace WebApp1.External.SmsRu;

public interface ISmsRuApiProvider
{
    Task<bool> SendSms(string phoneNumber, string message);
    Task<string?> MakePhoneCall(string phoneNumber, IPAddress? ip = null);
}