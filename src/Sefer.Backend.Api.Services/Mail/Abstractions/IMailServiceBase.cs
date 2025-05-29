namespace Sefer.Backend.Api.Services.Mail.Abstractions;

/// <summary>
/// The IMailService defines a service which is capable of sending e-mails.
/// This mail service uses views (using MailMessage)
/// </summary>
public interface IMailServiceBase
{
    /// <summary>
    /// Sends the e-mail as given in the message asynchronously using a queuing system
    /// </summary>
    /// <param name="message">The message to send</param>
    public void QueueEmailForSending(MailMessage message);

    /// <summary>
    /// Sends the e-mail as given in the message synchronously without a queue.
    /// Note: this method should only be used to testing or critical purposes!
    /// </summary>
    /// <param name="message">The message to send</param>
    public void SendEmailSynchronously(MailMessage message);
}
