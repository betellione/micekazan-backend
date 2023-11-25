using System.Net;
using WebApp1.External.SmsRu;

namespace WebApp1.Services.PhoneConfirmationService.PhoneCaller;

public class PhoneCaller : IPhoneCaller
{
    private readonly ISmsRuApiProvider _api;

    public PhoneCaller(ISmsRuApiProvider api)
    {
        _api = api;
    }

    public async Task<string> MakePhoneCall(string phoneNumber, IPAddress? ipAddress = null)
    {
        var code = await _api.MakePhoneCall(phoneNumber, ipAddress);
        return code ?? throw new Exception("Phone call was not made");
    }
}