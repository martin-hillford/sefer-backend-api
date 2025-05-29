namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostNameChangeChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_NoName()
    {
        (await Handle(null, [], -1)).Should().BeEmpty();
        (await Handle(string.Empty, [], -1)).Should().BeEmpty();
        (await Handle(" ", [], -1)).Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NoChannels()
    {
        var messages = await Handle("name", new List<Channel>(), 13);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var messages = await Handle("name", [channel], student.Id);
        messages.Should().NotBeEmpty();
    }
    
    [TestMethod]
    public async Task Handle_PostMessageNull()
    {
        var (channel, student, _) = await InitializePersonalChannel();
        var messages = await Handle("name", [channel], student.Id, false);
        messages.Should().BeEmpty();
    }

    private async Task<List<Message>> Handle(string? name, List<Channel> channels, int userId, bool inChannel = true)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetChannelsRequest, List<Channel>>(channels);
        provider.AddRequestResult<IsUserInChannelRequest, bool>(inChannel);
        if(channels.Count != 0) provider.AddRequestResult<GetChannelByIdRequest, Channel>(channels.First());
        var request = new PostNameChangeChatMessageRequest(name, userId);
        var handler = new PostNameChangeChatMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}