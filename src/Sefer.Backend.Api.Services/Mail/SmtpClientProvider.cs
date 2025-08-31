namespace Sefer.Backend.Api.Services.Mail;

public class SmtpClientProvider : ISmtpClientProvider
{
    public ISmtpClient GetSmtpClient() => new SmtpClient();
}

public interface ISmtpClientProvider
{
    ISmtpClient GetSmtpClient();
}