using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace FFMonitorWeb.Models {
    public class EmailSender : IEmailSender
{
    private readonly AuthMessageSenderOptions _options;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        return Execute(_options.SendGridKey, subject, message, email);
    }

    public Task Execute(string apiKey, string subject, string message, string email)
    {
        var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
            From = new EmailAddress("no-reply@yourdomain.com", "Your App"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(email));
        return client.SendEmailAsync(msg);
    }
}

public class AuthMessageSenderOptions
{
    public string SendGridKey { get; set; }
}

}
