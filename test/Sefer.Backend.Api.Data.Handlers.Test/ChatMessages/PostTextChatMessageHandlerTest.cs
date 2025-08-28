namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostTextChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider(new Exception());
        var request = new PostTextChatMessageRequest();
        var message = await Handle(request, provider);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_NotInChannel()
    {
        var request = new PostTextChatMessageRequest();
        var message = await Handle(request);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_NoQuotedText()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var request = new PostTextChatMessageRequest
        {
            Text = "text",
            ChannelId = channel.Id,
            SenderId = student.Id,
            QuotedMessageId = 19
        };
        var provider = GetProviderWithInChannel();
        var message = await Handle(request, provider);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_NoQuotedMessageFound()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var request = new PostTextChatMessageRequest
        {
            Text = "text",
            ChannelId = channel.Id,
            SenderId = student.Id,
            QuotedMessageId = 19,
            QuotedMessageText = "text"
        };
        var provider = GetProviderWithInChannel();
        var message = await Handle(request, provider);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var request = new PostTextChatMessageRequest
        {
            Text = "text",
            ChannelId = channel.Id,
            SenderId = student.Id,
        };
        var provider = GetProviderWithInChannel();
        var message = await Handle(request, provider);
        await VerifyMessage(message, false);
    }

    [TestMethod]
    public async Task Handle_WithQuotedMessage()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var quoted = new Message { ChannelId = channel.Id, SenderId = student.Id, ContentString = "message", Type = MessageTypes.Text };
        await InsertAsync(quoted);

        var request = new PostTextChatMessageRequest
        {
            Text = "text",
            ChannelId = channel.Id,
            SenderId = student.Id,
            QuotedMessageId = quoted.Id,
            QuotedMessageText = "quoted"
        };

        var provider = GetProviderWithInChannel();
        var message = await Handle(request, provider);
        await VerifyMessage(message, true);
    }

    private async Task VerifyMessage(Message message, bool withQuoted)
    {
        var context = GetDataContext();
        context.ChatChannelReceivers.Count().Should().Be(2);
        var student = await context.Users.SingleAsync(u => u.Role == UserRoles.Student);
        var mentor = await context.Users.SingleAsync(u => u.Role == UserRoles.Mentor);

        message.Should().NotBeNull();
        message.ContentString.Should().Be("text");
        message.SenderId.Should().Be(student.Id);
        message.Sender.Should().NotBeNull();
        message.Sender.Id.Should().Be(student.Id);
        message.IsAvailable.Should().BeTrue();
        message.ChannelMessages.Count.Should().Be(2);
        message.ChannelMessages.SingleOrDefault(m => m.ReceiverId == student.Id)?.Receiver.Should().NotBeNull();
        message.ChannelMessages.SingleOrDefault(m => m.ReceiverId == mentor.Id)?.Receiver.Should().NotBeNull();

        if (!withQuoted) return;

        var quotedMessage = context.ChatMessages.SingleOrDefault(m => m.Id == message.QuotedMessageId);
        quotedMessage.Should().NotBeNull();
        message.QuotedMessageString.Should().Be("quoted");
    }

    private MockedServiceProvider GetProviderWithInChannel()
    {
        var mocked = GetServiceProvider();
        mocked.AddRequestResults<IsUserInChannelRequest, bool>(true);
        return mocked;
    }

    private async Task<Message> Handle(PostTextChatMessageRequest request, MockedServiceProvider? provider = null)
    {
        var serviceProvider = provider ?? GetServiceProvider();
        var handler = new PostTextChatMessageHandler(serviceProvider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}