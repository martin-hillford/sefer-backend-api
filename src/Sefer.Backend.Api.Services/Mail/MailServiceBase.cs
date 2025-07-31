namespace Sefer.Backend.Api.Services.Mail;

/// <summary>
/// This MailService is a default implementation for mail service using an IViewRenderService
/// This Service is currently a wrapper for MailKit
/// </summary>
public class MailServiceBase(IOptions<MailServiceOptions> mailOptions, ILogger<MailServiceBase> logger) : IMailServiceBase
{
    private readonly MailServiceOptions _mailOptions = mailOptions.Value;

    private int _isRunning;

    private readonly ConcurrentQueue<MailMessage> _queue = new();

    /// <summary>
    /// Sends the e-mail as given in the message asynchronously using a queuing system
    /// </summary>
    /// <param name="message">The message to send</param>
    public void QueueEmailForSending(MailMessage message)
    {
        // Add the message to the queue
        _queue.Enqueue(message);

        // Check if the queue is being processed
        if (!IsQueueRunning()) StartQueueAsync();
    }

    private void StartQueueAsync() => Task.Run(StartQueue);

    private void StartQueue()
    {
        SetRunning(true);

        while (!_queue.IsEmpty)
        {
            var dequeued = _queue.TryDequeue(out var message);
            if (dequeued) SendEmail(message);
            Thread.Sleep(100);
        }

        SetRunning(false);
    }

    private void SetRunning(bool running)
    {
        Interlocked.Exchange(ref _isRunning, running ? 1 : 0);
    }

    private bool IsQueueRunning()
    {
        return Interlocked.CompareExchange(ref _isRunning, 0, 0) == 1;
    }

    /// <summary>
    /// This method can be used to send mail synchronously.
    /// </summary>
    /// <param name="message"></param>
    private void SendEmail(MailMessage message)
    {
        try { SendEmailSynchronously(message); }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error sending email");
        }
    }


    /// <summary>
    /// This method can be used to send mail synchronously.
    /// </summary>
    /// <param name="message"></param>
    public void SendEmailSynchronously(MailMessage message)
    {
        try
        {
            logger.LogInformation($"Sending message for view '{message.ViewIdentifier}'");

            // Fill all the basic header information
            var mimeMessage = GetMimeMessage(message);
            mimeMessage.From.Add(new MailboxAddress(message.SenderName, message.SenderEmail));

            // Now create the e-mail
            var builder = new BodyBuilder();

            // Create the message
            if (string.IsNullOrEmpty(message.Html) == false) builder.HtmlBody = message.Html.FixBareLineFeeds();
            if (string.IsNullOrEmpty(message.Text) == false) builder.TextBody = message.Text.FixBareLineFeeds();
            mimeMessage.Body = builder.ToMessageBody();
            
            // Check if the message does need to be written to a file
            WriteToFile(mimeMessage);
            
            // Check if e-mail delivery is enabled;
            if(!_mailOptions.Enabled) return;

            // Connect to the server and send the e-mail
            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("XOAUTH");
            
            client.Connect(_mailOptions.Host, _mailOptions.PortInt, _mailOptions.UseSslBoolean);
            client.Authenticate(_mailOptions.Username, _mailOptions.Password);

            client.Send(mimeMessage);
            client.Disconnect(true);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"Error occurred while sending e-mail '{message.ViewIdentifier}'");
            throw;
        }
    }

    private void WriteToFile(MimeMessage mimeMessage)
    {
        // Note: this should NOT interfere in any way with e-mail delivery
        try
        {
            if (string.IsNullOrEmpty(_mailOptions.WriteCopy)) return;
            if(!Directory.Exists(_mailOptions.WriteCopy)) return;

            var fileName = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + Guid.NewGuid().ToString() + ".txt";
            var fullName = Path.Combine(_mailOptions.WriteCopy, fileName);
            mimeMessage.WriteTo(fullName);    
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }

    private MimeMessage GetMimeMessage(MailMessage message)
    {
        var mine = new MimeMessage { Subject = message.Subject, Date = DateTime.UtcNow, Importance = MessageImportance.Normal, Priority = MessagePriority.Normal };
        if (!EnvVar.IsProductionEnv())
        {
            mine.To.Add(new MailboxAddress("admin", _mailOptions.AdminEmail));
        }
        else
        {
            foreach (var address in message.To.GetAddresses()) { mine.To.Add(new MailboxAddress(address.Name, address.Email)); }
        }
        return mine;
    }
}