using System.Threading.Channels;
using Channel = System.Threading.Channels.Channel;
using MailMessage = Sefer.Backend.Api.Services.Mail.Abstractions.MailMessage;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Sefer.Backend.Api.Services.Mail;

/// <summary>
/// This MailService is a default implementation for mail service using an IViewRenderService
/// This Service is currently a wrapper for MailKit
/// </summary>
public sealed class MailServiceBase : IMailServiceBase, IAsyncDisposable
{
    private readonly Channel<MailMessage> _channel;
    
    private readonly Task _consumerTask;
    
    private readonly CancellationTokenSource _cts = new();
    
    private readonly ILogger<MailServiceBase> _logger;
    
    private readonly MailServiceOptions _mailOptions;

    private readonly SmtpClient _client = new ();
    
    private readonly SemaphoreSlim _smtpGate = new(1, 1);
    
    private volatile bool _disposed;
    
    public MailServiceBase(IOptions<MailServiceOptions> mailOptions, ILogger<MailServiceBase> logger)
    {
        var channelOptions = new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<MailMessage>(channelOptions);
        _logger = logger;
        _mailOptions = mailOptions.Value;
        _client.Timeout = 10000;
        
        // Start background loop immediately
        _consumerTask = Task.Run(() => ProcessLoopAsync(_cts.Token));
    }
    
    
    public void QueueEmailForSending(MailMessage item)
    {
        // If the queue is disposed, don't add to the queue
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        // Add in a fire and forget mode. In this  
        EnqueueFireAndForget(item);
    }
    
    private void EnqueueFireAndForget(MailMessage item)
    {
        // Fast path
        if (_channel.Writer.TryWrite(item)) return;

        // Slow path: schedule the awaited write
        _ = _channel.Writer.WriteAsync(item, _cts.Token).AsTask().ContinueWith(t =>
        {
            if (t.IsCanceled) _logger.LogDebug("Enqueue canceled during shutdown.");
            else if (t.IsFaulted) _logger.LogError(t.Exception, "Background enqueue failed.");
        }, TaskContinuationOptions.ExecuteSynchronously);
    }
    
    private async Task ProcessLoopAsync(CancellationToken token)
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
            {
                while (_channel.Reader.TryRead(out var item))
                {
                    // Process the item. This method will take care of handling recoverable exceptions
                    await ProcessItem(item, token).ConfigureAwait(false);
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

    private async Task ProcessItem(MailMessage item, CancellationToken token)
    {
        var attempt = 0;
        for (;;)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                await SendEmailAsync(item, token).ConfigureAwait(false); // uses _client
                if (_mailOptions.IsRateLimit)
                {
                    await Task.Delay(_mailOptions.Delay, token).ConfigureAwait(false);
                }
                break;
            }
            catch (MailSendException exception)
            {
                attempt++;
                _logger.LogWarning(exception, "Send failed (attempt {Attempt}); resetting SMTP connection.", attempt);
                
                await SafeDisconnectAsync().ConfigureAwait(false);
                await Task.Delay(1000, token).ConfigureAwait(false);
                
                if (attempt < 10) continue;
                _logger.LogError(exception.InnerException, "Giving up after {Attempt} attempts for {View}.", attempt, item.ViewIdentifier);
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
            _client.AuthenticationMechanisms.Remove("XOAUTH");
            _logger.LogDebug("Connecting to server '{Host}'", _mailOptions.Host);
            if (!_client.IsConnected) await _client.ConnectAsync(_mailOptions.Host, _mailOptions.PortInt, _mailOptions.UseSslBoolean, token);
            
            // Perform proper authentication
            _logger.LogDebug("Connected to server '{Host}'", _mailOptions.Host);
            if (!_client.IsAuthenticated) await _client.AuthenticateAsync(_mailOptions.Username, _mailOptions.Password, token);

            // And send the e-mail
            _logger.LogDebug("Sending message '{MessageViewIdentifier}'", message.ViewIdentifier);
            await _client.SendAsync(mimeMessage, token);
        }
        catch (Exception exception)
        {
            // If an exception occurred during sending the e-mail, it is put back into the queue
            // But it can't be done in this method else deadlock can occur
            throw new MailSendException(exception);
        }
        finally
        {
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
            await mimeMessage.WriteToAsync(fullName, token);    
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
            foreach (var address in message.To.GetAddresses()) { mime.To.Add(new MailboxAddress(address.Name, address.Email)); }
        }
        return mime;
    }
    
    private async Task SafeDisconnectAsync()
    {
        // best-effort: avoid contending if the sender is active
        if (await _smtpGate.WaitAsync(0).ConfigureAwait(false))
        {
            try { if (_client.IsConnected) await _client.DisconnectAsync(true).ConfigureAwait(false); }
            catch { /* ignore */ }
            finally { _smtpGate.Release(); }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        // Prevent from calling disposed twice
        ObjectDisposedException.ThrowIf(_disposed, this);
        
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
    
    private class MailSendException(Exception innerException) : Exception("An SMTP Error occured", innerException);
}