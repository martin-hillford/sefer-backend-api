
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Sefer.Backend.Api.Services.Mail;
using Sefer.Backend.Api.Services.Mail.Abstractions;

namespace Sefer.Backend.Api.Services.Test.Mail;

[TestClass]
public class MailServiceBaseTest
{
    [TestMethod]
    public async Task QueueEmailForSending_DefaultOk()
    {
        var logger = new Mock<ILogger<MailServiceBase>>();
        var mailOptions = GetOptions();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(s => s.AuthenticationMechanisms).Returns([]);
        var smtpClientProvider = GetSmtpClientProvider(smtpClient);
        
        var mailer = new MailServiceBase(Options.Create(mailOptions), logger.Object, smtpClientProvider.Object);
        var mailMessage = GetMessage();
        
        mailer.QueueEmailForSending(mailMessage);
        await Task.Delay(200, TestContext.CancellationTokenSource.Token); // It is nasty to wait in a test but required
        
        smtpClient.Verify(s => s.AuthenticateAsync(mailOptions.Username, mailOptions.Password, It.IsAny<CancellationToken>()), Times.Once);
        smtpClient.Verify(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Once);
    }
    
    [TestMethod]
    public async Task QueueEmailForSending_Retries()
    {
        var logger = new Mock<ILogger<MailServiceBase>>();
        var mailOptions = GetOptions();
        var smtpClient = new Mock<ISmtpClient>();
        var smtpException = new Exception("Send failed");
        smtpClient.Setup(s => s.AuthenticationMechanisms).Returns([]);
        smtpClient
            .Setup(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null))
            .Throws(smtpException);
        var smtpClientProvider = GetSmtpClientProvider(smtpClient);
        
        var mailer = new MailServiceBase(Options.Create(mailOptions), logger.Object, smtpClientProvider.Object);
        var mailMessage = GetMessage();
        
        mailer.QueueEmailForSending(mailMessage);
        await Task.Delay(1000, TestContext.CancellationTokenSource.Token); // It is nasty to wait in a test but required
        
        smtpClient.Verify(s => s.AuthenticateAsync(mailOptions.Username, mailOptions.Password, It.IsAny<CancellationToken>()), Times.Exactly(10));
        smtpClient.Verify(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Exactly(10));
        
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Giving up after 10 attempts for test-view."),
                It.Is<Exception>(e => ReferenceEquals(e, smtpException)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [TestMethod]
    public async Task QueueEmailForSending_Disposed()
    {
        var logger = new Mock<ILogger<MailServiceBase>>();
        var mailOptions = GetOptions();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(s => s.AuthenticationMechanisms).Returns([]);
        var smtpClientProvider = GetSmtpClientProvider(smtpClient);
        
        var mailer = new MailServiceBase(Options.Create(mailOptions), logger.Object, smtpClientProvider.Object);
        await mailer.DisposeAsync();
        
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
        {
            mailer.QueueEmailForSending(GetMessage());
            return Task.CompletedTask;
        });
    }
    private static MailServiceOptions GetOptions(bool enabled = true, int maxPerMinute = 600, bool writeCopy = false)
    {
        return new MailServiceOptions
        {
            Enabled = enabled,
            AdminEmail = "admi@test.local",
            Host = "localhost",
            Port = "25",
            Username = "test",
            Password = "test",
            UseSsl = "false",
            WriteCopy = writeCopy ? "true" : "false",
            MaxMessagesPerMinute = maxPerMinute,
            DelayAfterException = 10
        };
    }

    private static MailMessage GetMessage()
    {
        return new MailMessage("reciever@testl.local","receiver")
        {
            Html = "<html><head></head><body><p>This is a test mail</p></body></html>",
            Subject = "This is a test subject",
            SenderEmail = "sender@test.local",
            SenderName = "sender",
            Text = "This is a test mail",
            ViewIdentifier = "test-view"
        };
    }

    private static Mock<ISmtpClientProvider> GetSmtpClientProvider(Mock<ISmtpClient> smtpClient)
    {
        var smtpClientProvider = new Mock<ISmtpClientProvider>();
        smtpClientProvider.Setup(x => x.GetSmtpClient()).Returns(smtpClient.Object);
        return smtpClientProvider;
    }
        
    public TestContext TestContext { get; set; }
}