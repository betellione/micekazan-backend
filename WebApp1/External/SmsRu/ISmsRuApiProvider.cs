namespace WebApp1.External.SmsRu;

public interface ISmsRuApiProvider
{
    public Task<bool> SendSms(string phoneNumber, string message);
}