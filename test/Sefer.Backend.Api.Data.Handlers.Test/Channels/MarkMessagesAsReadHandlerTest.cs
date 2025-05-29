namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class MarkMessagesAsReadHandlerTest : UnreadMessageUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var user = await context.Users.SingleAsync(u => u.Name == "userA");
        var channel = await context.ChatChannels.SingleAsync(u => u.Name == "channel1");
        var message = await context.ChatMessages.SingleAsync(u => u.ContentString == "1");

        var count = context.ChatChannelMessages.Count(c =>
            c.Message.ChannelId == channel.Id && c.ReceiverId == user.Id && c.ReadDate == null);
        Assert.AreEqual(2, count);

        var provider = GetServiceProvider();
        provider.AddRequestResult<IsUserInChannelRequest, bool>(true);
        var request = new MarkMessagesAsReadRequest(user.Id, channel.Id, [message.Id]);
        var handler = new MarkMessagesAsReadHandler(provider.Object);
        var updated = await handler.Handle(request, CancellationToken.None);

        Assert.IsTrue(updated);

        var recount = context.ChatChannelMessages.Count(c =>
            c.Message.ChannelId == channel.Id && c.ReceiverId == user.Id && c.ReadDate == null);
        Assert.AreEqual(1, recount);
    }
}