using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApp1.Options;

namespace WebApp1.Services.EmailSender;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public EmailSender(IOptions<SmtpOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        using var client = new SmtpClient();
        var msg = new MimeMessage
        {
            From = { new MailboxAddress(_options.Username, _options.Address), },
            Subject = subject,
            Body = new TextPart(TextFormat.Html) { Text = message, },
            To = { new MailboxAddress(string.Empty, toEmail), },
        };

        await client.ConnectAsync(_options.Host, _options.Port, true);
        await client.AuthenticateAsync(_options.Address, _options.Password);
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}