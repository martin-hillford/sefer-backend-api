namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostObjectChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_Exception()
    {
        var request = new PostObjectChatMessageRequest { ReferenceId = 1 };
        var channel = new Channel();
        var message = await Handle(request, true, channel);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_ReferenceMissing()
    {
        var request = new PostObjectChatMessageRequest { ReferenceId = 0 };
        var message = await Handle(request, true, null);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_NotInChannel()
    {
        var request = new PostObjectChatMessageRequest { ReferenceId = 1 };
        var message = await Handle(request, false, null);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle_ChannelNull()
    {
        var request = new PostObjectChatMessageRequest { ReferenceId = 1 };
        var message = await Handle(request, true, null);
        Assert.IsNull(message);
    }

    [TestMethod]
    public async Task Handle()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var blog = new Blog { Id = 1, Content = "content" };
        var request = new PostObjectChatMessageRequest
        {
            ReferenceId = blog.Id,
            Message = blog,
            Type = MessageTypes.Text,
            ChannelId = channel.Id,
            SenderId = student.Id
        };

        var message = await Handle(request, true, channel);
        message.Should().NotBeNull();
        message.ContentString.Should().NotBeNullOrEmpty();
        message.SenderDate.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));

        var context = GetDataContext();
        context.ChatChannelMessages.Count().Should().Be(2);

        var sender = context.ChatChannelMessages.Single(c => c.ReceiverId == student.Id);
        Assert.IsNotNull(sender);
        Assert.IsNotNull(sender.ReadDate);
        sender.ReadDate.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
    }

    private async Task<Message> Handle(PostObjectChatMessageRequest request, bool inChannel, Channel? channel)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsUserInChannelRequest, bool>(inChannel);
        if(channel != null) provider.AddRequestResult<GetChannelByIdRequest, Channel>(channel);
        var handler = new PostObjectChatMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}