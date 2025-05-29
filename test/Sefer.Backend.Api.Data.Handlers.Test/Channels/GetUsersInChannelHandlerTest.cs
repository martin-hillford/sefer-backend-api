namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetUsersInChannelHandlerTest : UsersInChannelUnitTest
{
    [TestMethod]
    [DataRow("channel1", "userA", "userB")]
    [DataRow("channel2", "userA", "userC")]
    [DataRow("channel3", "userA")]
    public async Task Handle(string channelName, params string[] userNames)
    {
        var context = GetDataContext();
        var channel = await context.ChatChannels.SingleAsync(u => u.Name == channelName);

        var request = new GetUsersInChannelRequest(channel.Id);
        var handler = new GetUsersInChannelHandler(ServiceProvider);
        var users = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(userNames.Length, users.Count);
        Assert.IsTrue(users.All(u => userNames.Contains(u.Name)));

    }
}