using System.Net;

namespace WebApp1.Services.PhoneConfirmationService;

public interface IPhoneConfirmationService
{
    public Task<bool> MakePhoneCallWithToken(Guid userId, string phoneNumber, string confirmationToken, IPAddress? ip = null);
    public Task<string> GetConfirmationTokenForUser(Guid userId, string phoneNumber, string code);
}