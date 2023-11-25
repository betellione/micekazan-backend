using System.Net;

namespace WebApp1.Services.PhoneConfirmationService.PhoneCaller;

public interface IPhoneCaller
{
    /// <summary>
    /// Make a call to the given phone number.
    /// </summary>
    /// <param name="phoneNumber">Phone number to call.</param>
    /// <param name="ipAddress">IP address of the remote client to prevent multiple calls in a short time.</param>
    /// <returns>The last 4 digits of the number from which the call was made.</returns>
    /// <remarks>Not all providers support IP address so it is optional.</remarks>
    /// <exception cref="Exception">Phone call was not made due to provider's error or invalid input data.</exception>
    public Task<string> MakePhoneCall(string phoneNumber, IPAddress? ipAddress = null);
}