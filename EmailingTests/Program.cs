using Microsoft.Extensions.Options;
using WebApp1.Options;
using WebApp1.Services.EmailSender;

var options = new SmtpOptions
{
    Address = Environment.GetEnvironmentVariable("ET_ADDRESS") ?? string.Empty,
    Host = Environment.GetEnvironmentVariable("ET_HOST") ?? string.Empty,
    Password = Environment.GetEnvironmentVariable("ET_PASSWORD") ?? string.Empty,
    Port = 465,
    Username = Environment.GetEnvironmentVariable("ET_USERNAME") ?? string.Empty,
};

var sender = new EmailSender(Options.Create(options));
var to = Environment.GetEnvironmentVariable("ET_ADDRESS_TO") ?? string.Empty;

await sender.SendEmailAsync(to, "Subject", "Message");

Console.WriteLine("Done.");