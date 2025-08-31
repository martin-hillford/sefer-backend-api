using System.Threading.Channels;
using Channel = System.Threading.Channels.Channel;
using MailMessage = Sefer.Backend.Api.Services.Mail.Abstractions.MailMessage;

namespace Sefer.Backend.Api.Services.Mail;

/// <summary>
/// This service will send mail message (current via a smtp relay). It ensures thread safety and will limit the
/// smtp connection to only one concurrent smtp connection (since many smtp server are rate limited) 
/// </summary>
public sealed class MailServiceBase : IMailServiceBase, IAsyncDisposable
{
    private readonly Channel<MailMessage> _channel;
    
    private readonly Task _consumerTask;
    
    private readonly CancellationTokenSource _cts = new();
    
    private readonly ILogger<MailServiceBase> _logger;
    
    private readonly MailServiceOptions _mailOptions;

    private readonly ISmtpClient _client;
    
    private readonly SemaphoreSlim _smtpGate = new(1, 1);
    
    private volatile bool _disposed;
    
    private const int MaxRetries = 10;
    
    public MailServiceBase(IOptions<MailServiceOptions> mailOptions, ILogger<MailServiceBase> logger, ISmtpClientProvider smtpClientProvider)
    {
        // Store the required helpers
        _logger = logger;
        _mailOptions = mailOptions.Value;
        
        // Create a bounded channel that acts as a queue. Only one reader can be active as only one smtp connection
        // is made to prevent the smtp server from being overwhelmed by smtp connections (or being kicked by a rate
        // limiter at the smtp server)
        var channelOptions = new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<MailMessage>(channelOptions);
        
        // Create the smtp client
        _client = smtpClientProvider.GetSmtpClient();
        _client.AuthenticationMechanisms.Remove("XOAUTH");
        _client.Timeout = 10000;
        
        // Start background loop immediately.
        // NB. this must be the last action in the constructor to prevent race conditions 
        _consumerTask = Task.Run(() => HandleChannelAsync(_cts.Token));
    }
    
    public void QueueEmailForSending(MailMessage message)
    {
        // If the queue is disposed, don't add to the queue
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        // Add the message in a fire and forget style.
        // In this way callers will not be blocked when the queue is full  
        EnqueueFireAndForget(message);
    }
    
    private void EnqueueFireAndForget(MailMessage item)
    {
        // Fast path (the queue is not locked and there is space in the queue)
        if (_channel.Writer.TryWrite(item)) return;

        // Slow path: schedule the awaited write. (run this in a fire and forget style)
        _ = _channel.Writer.WriteAsync(item, _cts.Token).AsTask().ContinueWith(t =>
        {
            if (t.IsCanceled) _logger.LogDebug("Enqueue canceled during shutdown.");
            else if (t.IsFaulted) _logger.LogError(t.Exception, "Background enqueue failed.");
        }, TaskContinuationOptions.ExecuteSynchronously);
    }
    
    private async Task HandleChannelAsync(CancellationToken token)
    {
        try
        {
            // Keep active and wait for the channel to receive a message
            while (await _channel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
            {
                // Try to get the next mail message from the queue and process it
                while (_channel.Reader.TryRead(out var message))
                {
                    // Process the item. This method will take care of handling recoverable exceptions
                    await HandleMailMessageAsync(message, token).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) { /* shutting down */ }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail queue crashed, this is unrecoverable error.");
        }
        finally
        {
            await SafeDisconnectAsync().ConfigureAwait(false);
        }
    }

    private async Task HandleMailMessageAsync(MailMessage message, CancellationToken token)
    {
        // Since Smtp errors can occur, this handler will retry to resend the message as smtp exceptions occur
        var attempt = 0;
        for (;;)
        {
            // If the process is being cannel, do not continue (e.q. at shutdown)
            token.ThrowIfCancellationRequested();
            try
            {
                // Try to send the e-mail 
                await SendEmailAsync(message, token).ConfigureAwait(false);
                
                // If the options indicate rate limiting is enabled delay the task
                if (_mailOptions.IsRateLimit) await Task.Delay(_mailOptions.Delay, token).ConfigureAwait(false);
                break; // Break out of the for loop
            }
            // A smtp connection has occured
            catch (MailSendException exception)
            {
                attempt++;
                _logger.LogWarning(exception, "Send failed (attempt {Attempt}); resetting SMTP connection.", attempt);
                
                // Ensure to disconnect from the server, wait for 5 seconds and retry to send the message
                await SafeDisconnectAsync().ConfigureAwait(false);
                await Task.Delay(_mailOptions.DelayAfterException ?? 5000, token).ConfigureAwait(false);
                
                // If the max number of retries has not been reached,
                // simple continue the for loop, else give up and break
                if (attempt < MaxRetries) continue;
                _logger.LogError(exception.InnerException, "Giving up after {Attempt} attempts for {View}.", attempt, message.ViewIdentifier);
                break;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Send failed; resetting SMTP connection.");
                await SafeDisconnectAsync().ConfigureAwait(false); // simple reset
                break; // Unknown what happened, break out of the loop safely
            }
        }
    }
    
    private async Task SendEmailAsync(MailMessage message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending message for view '{MessageViewIdentifier}'", message.ViewIdentifier);
            
            // Fill all the basic header information
            var mimeMessage = GetMimeMessage(message);
            mimeMessage.From.Add(new MailboxAddress(message.SenderName, message.SenderEmail));

            // Now create the e-mail
            var builder = new BodyBuilder();

            // Create the message
            if (!string.IsNullOrEmpty(message.Html)) builder.HtmlBody = message.Html.FixBareLineFeeds();
            if (!string.IsNullOrEmpty(message.Text)) builder.TextBody = message.Text.FixBareLineFeeds();
            mimeMessage.Body = builder.ToMessageBody();
            
            // Check if the message does need to be written to a file
            _logger.LogDebug("Writing message to file if required");
            await WriteToFile(mimeMessage, cancellationToken);
            
            // Check if e-mail delivery is enabled and is enabled send the message 
            _logger.LogDebug("Check if e-mail delivery is enabled: {enabled}", _mailOptions.Enabled);
            if (!_mailOptions.Enabled) return;
            await SendSmtpMessage(message, mimeMessage, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while sending e-mail '{MessageViewIdentifier}'", message.ViewIdentifier);
            throw;
        }
    }

    private async Task SendSmtpMessage(MailMessage message, MimeMessage mimeMessage, CancellationToken token)
    {
        // Absolutely ensure that this is thread safe
        await _smtpGate.WaitAsync(token).ConfigureAwait(false);
        try
        {
            // Connect to the server
            _logger.LogDebug("Connecting to server '{Host}'", _mailOptions.Host);
            await SmtpConnectAsync(token).ConfigureAwait(false);
            
            // Perform proper authentication
            _logger.LogDebug("Connected to server '{Host}'", _mailOptions.Host);
            await SmtpAuthenticateAsync(token).ConfigureAwait(false);
            
            // And send the e-mail
            _logger.LogDebug("Sending message '{MessageViewIdentifier}'", message.ViewIdentifier);
            await _client.SendAsync(mimeMessage, token);
        }
        catch (OperationCanceledException) // ← let cancellation flow
        {
            throw;
        }
        catch (Exception exception)
        {
            // If an exception occurred during sending the e-mail, inform the HandleMailMessageAsync about 
            // the exception so that sending the message can be retried
            throw new MailSendException(exception);
        }
        finally
        {
            // Ensure to release the lock so that the smtp client is released as well
            _smtpGate.Release();
        }
    }
    
    private async Task WriteToFile(MimeMessage mimeMessage, CancellationToken token)
    {
        // Note: this should NOT interfere in any way with e-mail delivery
        try
        {
            if (string.IsNullOrEmpty(_mailOptions.WriteCopy)) return;
            if(!Directory.Exists(_mailOptions.WriteCopy)) return;

            var fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + Guid.NewGuid() + ".txt";
            var fullName = Path.Combine(_mailOptions.WriteCopy, fileName);
            await mimeMessage.WriteToAsync(fullName, token).ConfigureAwait(false);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }

    private MimeMessage GetMimeMessage(MailMessage message)
    {
        var mime = new MimeMessage { Subject = message.Subject, Date = DateTime.UtcNow, Importance = MessageImportance.Normal, Priority = MessagePriority.Normal };
        if (!EnvVar.IsProductionEnv())
        {
            mime.To.Add(new MailboxAddress("admin", _mailOptions.AdminEmail));
        }
        else
        {
            foreach (var address in message.To.GetAddresses())
            {
                mime.To.Add(new MailboxAddress(address.Name, address.Email));
            }
        }
        return mime;
    }
    
    private async Task SafeDisconnectAsync()
    {
        // best-effort: avoid contending if the sender is active
        if (await _smtpGate.WaitAsync(1000).ConfigureAwait(false))
        {
            try { if (_client.IsConnected) await _client.DisconnectAsync(true).ConfigureAwait(false); }
            catch { /* ignore */ }
            finally { _smtpGate.Release(); }
        }
    }

    private async Task SmtpConnectAsync(CancellationToken token)
    {
        if (_client.IsConnected) return;
        await _client.ConnectAsync(_mailOptions.Host, _mailOptions.PortInt, _mailOptions.UseSslBoolean, token)
            .ConfigureAwait(false);
    }

    private async Task SmtpAuthenticateAsync(CancellationToken token)
    {
        if (_client.IsAuthenticated) return;
        await _client.AuthenticateAsync(_mailOptions.Username, _mailOptions.Password, token).ConfigureAwait(false);
    }
    
    public async ValueTask DisposeAsync()
    {
        // Prevent from calling disposed twice
        if (_disposed) return;
        
        // Mark that the disposed service is called, this prevents from new message coming into the queue
        _disposed = true;
        
        // Try to complete the queue
        _channel.Writer.TryComplete();
        
        // Give it time to finish gracefully.
        var graceful = await Task.WhenAny(_consumerTask, Task.Delay(TimeSpan.FromSeconds(10))).ConfigureAwait(false);
        if (graceful != _consumerTask)
        {
            // Force stop if still busy (e.g., hung SMTP).
            await _cts.CancelAsync().ConfigureAwait(false);
            try { await _consumerTask.ConfigureAwait(false); } catch { /* already logged */ }
        }
        
        // Check if any connection is still open
        await SafeDisconnectAsync().ConfigureAwait(false);
        
        // Dispose the remaining objects
        _cts.Dispose();
        _client.Dispose();
    }
    
    private sealed class MailSendException(Exception innerException) : Exception("An SMTP Error occurred", innerException);
}