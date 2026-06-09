namespace BidaPlatform.Domain.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
