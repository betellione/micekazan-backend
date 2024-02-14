namespace WebApp1.Services.EmailSender;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}