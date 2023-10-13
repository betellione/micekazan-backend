using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApp1.Options;

namespace WebApp1.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options; //Set with Secret Manager.

    public EmailSender(IOptions<SmtpOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        await Execute(_options, subject, message, toEmail);
    }

    private static async Task Execute(SmtpOptions smtpOptions, string subject, string message, string toEmail)
    {
        var msg = new MimeMessage
        {
            From = { new MailboxAddress(smtpOptions.Username, smtpOptions.Address), },
            Subject = subject,
            Body = new TextPart(TextFormat.Html) { Text = message, },
            To = { new MailboxAddress("", toEmail), },
        };
        using var client = new SmtpClient();
        await client.ConnectAsync(smtpOptions.Host, smtpOptions.Port, true);

        // Note: only needed if the SMTP server requires authentication
        await client.AuthenticateAsync(smtpOptions.Address, smtpOptions.Password);

        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}