namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetUnreadMessageCountForChannelHandlerTest : UnreadMessageUnitTest
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

        var request = new GetUnreadMessageCountForChannelRequest(user.Id, channel.Id);
        var handler = new GetUnreadMessageCountForChannelHandler(ServiceProvider);
        var count = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(unread, count);
    }
}