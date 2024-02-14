namespace WebApp1.Services.SmsSender;

public interface ISmsSender
{
    /// <summary>
    /// Send an SMS to the given phone number.
    /// </summary>
    /// <param name="number">Phone number to which the SMS will be sent.</param>
    /// <param name="message">Message to send.</param>
    /// <returns></returns>
    /// <exception cref="Exception">SMS was not sent due to provider's error or invalid input data.</exception>
    Task SendSmsAsync(string number, string message);
}