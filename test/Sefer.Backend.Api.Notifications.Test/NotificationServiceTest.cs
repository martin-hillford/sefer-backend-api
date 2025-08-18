using System.Text.Json;
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Shared;

namespace Sefer.Backend.Api.Notifications.Test;

[TestClass]
public class NotificationServiceTest
{
    [TestMethod]
    public async Task SendProfileUpdatedNotificationAsync_MultipleMessages()
    {
        // Arrange
        const int senderId = 100; const string oldName = "old-student";  const int mentorId = 23;
        var message = CreateMessage(MessageTypes.Text, senderId, mentorId);
        var messages = new List<Message> { message };
        var socket = new Mock<IWebSocketProvider>();

        var serviceProvider = new MockedServiceProvider()
            .AddRequestResult<PostNameChangeChatMessageRequest, List<Message>>(messages)
            .AddRequestResult<LoadMessageReferencesRequest, bool>(true)
            .AddService(socket);

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendProfileUpdatedNotificationAsync(senderId, oldName);

        // Assess
        socket.Verify(s => s.SendMessage(It.IsAny<MessageView>(), false), Times.Exactly(2));
    }

    [TestMethod]
    public async Task SendChatMessageSendNotificationAsync_ForSender()
    {
        // Arrange
        var message = CreateMessage(MessageTypes.Text, 10, 23);
        var senderMessage = message.ChannelMessages.First(c => c.Receiver.Name == "sender");
        var socket = new Mock<IWebSocketProvider>();
        var push = new Mock<IFireBaseService>();
        var serviceProvider = new MockedServiceProvider()
            .AddService(push)
            .AddService(socket);

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendChatMessageSendNotificationAsync(senderMessage);

        // Assess
        socket.Verify(s => s.SendMessage(It.IsAny<MessageView>(), false), Times.Once);
        push.Verify(p => p.SendChatTextMessageNotification(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
    }

    [TestMethod]
    [DataRow(MessageTypes.Text, 1)]
    [DataRow(MessageTypes.NameChange, 0)]
    public async Task SendChatMessageSendNotificationAsync_ForReceiver(MessageTypes messageType, int times)
    {
        // Arrange
        var message = CreateMessage(messageType, 10, 23);
        var senderMessage = message.ChannelMessages.First(c => c.Receiver.Name == "receiver");
        var socket = new Mock<IWebSocketProvider>();
        var push = new Mock<IFireBaseService>();
        var digest = new Mock<IEmailDigestService>();
        var serviceProvider = new MockedServiceProvider()
            .AddRequestResult<GetUserByIdRequest, User>(senderMessage.Receiver)
            .AddService(push)
            .AddService(digest)
            .AddService(socket);
        senderMessage.Receiver.NotificationPreference = NotificationPreference.Direct;

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendChatMessageSendNotificationAsync(senderMessage);

        // Assess
        socket.Verify(s => s.SendMessage(It.IsAny<MessageView>(), false), Times.Once);
        push.Verify(p => p.SendChatTextMessageNotification(senderMessage.ReceiverId, "sender", message.ContentString, false), Times.Exactly(times));
        // digest.Verify(d => d.SendNotificationsDigestAsync(senderMessage.Receiver));
    }

    [TestMethod]
    public async Task SendLessonSubmittedNotificationAsync_MessageNull()
    {
        // Arrange
        var serviceProvider = new MockedServiceProvider();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        var result = await service.SendLessonSubmittedNotificationAsync(13, null, null);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task SendLessonSubmittedNotificationAsync_StudentNull()
    {
        // Arrange
        var serviceProvider = new MockedServiceProvider();
        serviceProvider.AddRequestResult<PostSubmissionMessageRequest, Message>(new Message());
        var mentor = new User();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        var result = await service.SendLessonSubmittedNotificationAsync(13, mentor, null);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task SendLessonSubmittedNotificationAsync_MentorNull()
    {
        // Arrange
        var serviceProvider = new MockedServiceProvider();
        serviceProvider.AddRequestResult<PostSubmissionMessageRequest, Message>(new Message());
        var student = new User();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        var result = await service.SendLessonSubmittedNotificationAsync(13, null, student);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(NotificationPreference.Direct, 1)]
    [DataRow(NotificationPreference.None, 0)]
    public async Task SendLessonSubmittedNotificationAsync(NotificationPreference pref, int times)
    {
        // Arrange
        var student = new User { Id = 13, Name = "student" };
        var mentor = new User { Id = 27, NotificationPreference = pref, Name = "mentor" };
        var submissionId = 37;
        var message = CreateMessage(MessageTypes.StudentLessonSubmission, student.Id, mentor.Id);
        message.ContentString = JsonSerializer.Serialize(new SubmissionView(), DefaultJsonOptions.GetOptions());

        var mailService = new Mock<IEmailDigestService>();
        var socketService = new Mock<IWebSocketProvider>();
        var pushService = new Mock<IFireBaseService>();
        var serviceProvider = new MockedServiceProvider()
            .AddRequestResult<PostSubmissionMessageRequest, Message>(message)
            .AddService(pushService)
            .AddService(socketService)
            .AddService(mailService);

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendLessonSubmittedNotificationAsync(submissionId, mentor, student);

        // Assess - NB: the NotificationService will not kick the direct notification service anymore at this point
        // Todo: ensure the direct notification service does not require a background task anymore
        // mailService.Verify(s => s.SendNotificationsDigestAsync(mentor), Times.Exactly(times));
        socketService.Verify(s => s.SendMessage(It.Is<MessageView>(m => m.UserId == mentor.Id), false), Times.Once);
        socketService.Verify(s => s.SendMessage(It.Is<MessageView>(m => m.UserId == student.Id), false), Times.Once);
        pushService.Verify(s => s.SendLessonSubmittedNotificationToMentor(submissionId, mentor, student));
    }

    [TestMethod]
    public async Task SendChatMessageIsReadNotificationAsync()
    {
        // Arrange
        var socket = new Mock<IWebSocketProvider>();
        var serviceProvider = new MockedServiceProvider().AddService(socket);
        var user = new User();
        var messageId = 37;

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendChatMessageIsReadNotificationAsync(messageId, user);

        // Assess
        socket.Verify(s => s.MarkMessageRead(messageId, user), Times.Once);
    }

    [TestMethod]
    public async Task SendStudentIsInactiveNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();
        var push = new Mock<IFireBaseService>();
        serviceProvider.AddService(push);

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendStudentIsInactiveNotificationAsync(user);

        // Assess
        mailService.Verify(m => m.SendStudentIsInactiveEmailAsync(It.IsAny<MailData>()), Times.Once);
        push.Verify(m => m.SendStudentIsInactiveNotificationAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task SendCompleteRegistrationNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendCompleteRegistrationNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendCompleteRegistrationEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendPasswordForgotNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendPasswordForgotNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendPasswordForgotEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendPasswordResetCompletedNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendPasswordResetCompletedNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendPasswordResetCompletedEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendEmailUpdateRequestedNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendEmailUpdateRequestedNotificationAsync(user, "nl", "new@email.tld");

        // Assess
        mailService.Verify(m => m.SendEmailUpdateRequestedToNewAddressAsync(It.IsAny<MailData>(), "new@email.tld"), Times.Once);
        mailService.Verify(m => m.SendEmailUpdateRequestedToOldAddressAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendEmailUpdateCompleteNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendEmailUpdateCompleteNotificationAsync(user, "nl", "new@email.tld", "old@email.tld");

        // Assess
        mailService.Verify(m => m.SendEmailUpdateCompleteToNewAddressAsync(It.IsAny<MailData>(), "new@email.tld", "old@email.tld"), Times.Once);
        mailService.Verify(m => m.SendEmailUpdateCompleteToOldAddressAsync(It.IsAny<MailData>(), "new@email.tld", "old@email.tld"), Times.Once);
    }

    [TestMethod]
    public async Task SendAccountDeleteConfirmationNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendAccountDeleteConfirmationNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendAccountDeleteConfirmationEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendRewardReceivedNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();
        var rewards = new List<RewardGrant>();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendRewardReceivedNotificationAsync(user, rewards);

        // Assess
        mailService.Verify(m => m.SendRewardReceivedEmailAsync(It.IsAny<MailData>(), rewards), Times.Once);
    }

    [TestMethod]
    public async Task SendTwoFactorAuthDisabledNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendTwoFactorAuthDisabledNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendTwoFactorAuthDisabledEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    [TestMethod]
    public async Task SendTwoFactorAuthEnabledNotificationAsync()
    {
        // Arrange
        var (serviceProvider, mailService, user) = GetMockedMailService();

        // Act
        var service = new NotificationService(serviceProvider.Object);
        await service.SendTwoFactorAuthEnabledNotificationAsync(user, "nl");

        // Assess
        mailService.Verify(m => m.SendTwoFactorAuthEnabledEmailAsync(It.IsAny<MailData>()), Times.Once);
    }

    private static (MockedServiceProvider, Mock<IMailService>, User) GetMockedMailService()
    {
        var mailService = new Mock<IMailService>();
        var site = new Mock<ISite>();
        site.SetupGet(s => s.SiteUrl).Returns("https://test.tld");
        site.SetupGet(s => s.Hostname).Returns("test.tld");
        var serviceProvider = new MockedServiceProvider()
            .AddService(mailService)
            .AddRequestResult<GetSiteByNameRequest, ISite>(site.Object);
        var user = new User { Id = 13, PrimaryRegion = "nl", PrimarySite = "test.tld" };
        return (serviceProvider, mailService, user);
    }

    private static Message CreateMessage(MessageTypes type, int senderId, int receiverId)
    {
        var sender = new User { Name = "sender", Id = senderId };
        var receiver = new User { Name = "receiver", Id = receiverId };

        var channel = new Channel
        {
            Id = 10,
            Type = ChannelTypes.Personal,
            Receivers =
            [
                new ChannelReceiver { ChannelId = 10, UserId = sender.Id, User = sender },
                new ChannelReceiver { ChannelId = 10, UserId = receiver.Id, User = receiver },
            ]
        };

        var message = new Message
        {
            SenderId = senderId,
            Type = type,
            ChannelId = 10,
            ChannelMessages =
            [
                new ChannelMessage { ReceiverId = sender.Id, Receiver = sender  },
                new ChannelMessage { ReceiverId = receiver.Id, Receiver = receiver }
            ],
            Channel = channel,
            ContentString = "content",
            Sender = sender
        };
        foreach (var channelMessage in message.ChannelMessages) { channelMessage.Message = message; }
        foreach (var channelReceiver in channel.Receivers) { channelReceiver.Channel = channel; }
        return message;
    }
}