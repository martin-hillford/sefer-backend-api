namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class IsUserInChannelHandlerTest : UsersInChannelUnitTest
{
    [TestMethod]
    [DataRow("userA", "channel1", true)]
    [DataRow("userA", "channel2", true)]
    [DataRow("userA", "channel3", true)]
    [DataRow("userB", "channel1", true)]
    [DataRow("userB", "channel2", false)]
    [DataRow("userB", "channel3", false)]
    [DataRow("userC", "channel1", false)]
    [DataRow("userC", "channel2", true)]
    [DataRow("userC", "channel3", false)]
    [DataRow("userD", "channel1", false)]
    [DataRow("userD", "channel2", false)]
    [DataRow("userD", "channel3", false)]
    public async Task Handle(string userName, string channelName, bool expected)
    {
        var context = GetDataContext();
        var user = await context.Users.SingleAsync(u => u.Name == userName);
        var channel = await context.ChatChannels.SingleAsync(u => u.Name == channelName);

        var request = new IsUserInChannelRequest(channel.Id, user.Id);
        var handler = new IsUserInChannelHandler(ServiceProvider);
        var inChannel = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(expected, inChannel);
    }
}