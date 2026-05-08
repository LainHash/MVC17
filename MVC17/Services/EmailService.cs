using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace MVC17.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["GmailSettings:SenderName"], _config["GmailSettings:SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            // Use smtp.gmail.com and 587 instead of the wrong ones from appsettings if needed, 
            // but we will read from config assuming it is fixed.
            var host = _config["GmailSettings:Host"];
            var port = int.Parse(_config["GmailSettings:Port"]!);
            
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["GmailSettings:SenderEmail"], _config["GmailSettings:AppPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
