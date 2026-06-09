using System.Net;
using System.Net.Mail;
using BidaPlatform.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BidaPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var fromEmail = _config["Email:From"];
        var fromName = _config["Email:FromName"];

        var smtp = new SmtpClient(_config["Email:SmtpHost"])
        {
            Port = int.Parse(_config["Email:SmtpPort"]!),
            Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail!, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mail.To.Add(to);

        await smtp.SendMailAsync(mail);
    }
}
