namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetUnreadMessageCountHandlerTest : UnreadMessageUnitTest
{
    [TestMethod]
    [DataRow("userA", "channel1", 2, 2)]
    [DataRow("userA", "channel2", 1, 2)]
    public async Task Handle(string userName, string channelName, int unread, int channelCount)
    {
        var context = GetDataContext();
        var user = await context.Users.SingleAsync(u => u.Name == userName);
        var channel = await context.ChatChannels.SingleAsync(u => u.Name == channelName);

        var request = new GetUnreadMessageCountRequest(user.Id);
        var handler = new GetUnreadMessageCountHandler(ServiceProvider);
        var dictionary = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(channelCount, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey(channel.Id));
        Assert.AreEqual(unread, dictionary[channel.Id]);
    }
}