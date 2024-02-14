using System.Net;

namespace WebApp1.Services.PhoneConfirmationService;

public interface IPhoneConfirmationService
{
    Task<bool> MakePhoneCallWithToken(Guid userId, string phoneNumber, string confirmationToken, IPAddress? ip = null);
    Task<string> GetConfirmationTokenForUser(Guid userId, string phoneNumber, string code);
}