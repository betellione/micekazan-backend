namespace WebApp1.Services.SmsSender;

public interface ISmsSender
{
    public Task SendSmsAsync(string number, string message);
}