namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class MarkChannelAsReadHandlerTest : UnreadMessageUnitTest
{
    [TestMethod]
    [DataRow("userA", "channel1", 2)]
    [DataRow("userB", "channel1", 2)]
    [DataRow("userA", "channel2", 1)]
    [DataRow("userC", "channel2", 1)]
    public async Task Handle(string userName, string channelName, int unread)
    {
        var context = GetDataContext();
        var user = await context.Users.SingleAsync(u => u.Name == userName);
        var channel = await context.ChatChannels.SingleAsync(u => u.Name == channelName);

        var count = context.ChatChannelMessages.Count(c => c.Message.ChannelId == channel.Id && c.ReceiverId == user.Id && c.ReadDate == null);
        Assert.AreEqual(unread, count);

        var provider = GetServiceProvider();
        provider.AddRequestResult<IsUserInChannelRequest, bool>(true);
        var request = new MarkChannelAsReadRequest(user.Id, channel.Id);
        var handler = new MarkChannelAsReadHandler(provider.Object);
        var updated = await handler.Handle(request, CancellationToken.None);

        Assert.IsTrue(updated);

        var recount = context.ChatChannelMessages.Count(c => c.Message.ChannelId == channel.Id && c.ReceiverId == user.Id && c.ReadDate == null);
        Assert.AreEqual(0, recount);
    }

    [TestMethod]
    public async Task Handle_UserNotInChannel()
    {
        var request = new MarkChannelAsReadRequest(1, 1);
        var handler = new MarkChannelAsReadHandler(ServiceProvider);
        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(updated);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider();
        provider.AddRequestException<IsUserInChannelRequest, bool>();
        var request = new MarkChannelAsReadRequest(1, 1);
        var handler = new MarkChannelAsReadHandler(provider.Object);
        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(updated);
    }
}