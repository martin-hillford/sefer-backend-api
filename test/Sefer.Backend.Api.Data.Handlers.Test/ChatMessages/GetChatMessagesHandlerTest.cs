namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class GetChatMessagesHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InitializeMessages();
    }

    [TestMethod]
    public async Task Handle_TakeZero()
    {
        var context = GetDataContext();
        var channel = context.ChatChannels.Single();

        var result = await Handle(channel.Id, 0, 0);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task Handle_NotIsAvailable()
    {
        var context = GetDataContext();
        var channel = context.ChatChannels.Single();
        var message = context.ChatMessages.ToList().Last();
        message.IsAvailable = false;
        context.UpdateSingleProperty(message, nameof(message.IsAvailable));

        var result = await Handle(channel.Id, 0, 10);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var channel = context.ChatChannels.Single();

        var result = await Handle(channel.Id, 0, 10);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("A", result.First().ContentString);
    }

    private async Task<List<Message>> Handle(int channelId, int skip, int take)
    {
        var request = new GetChatMessagesRequest(channelId, skip, take);
        var handler = new GetChatMessagesHandler(ServiceProvider);
        return await handler.Handle(request, CancellationToken.None);
    }
}