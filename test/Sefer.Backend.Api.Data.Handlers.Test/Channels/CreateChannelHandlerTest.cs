namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class CreateChannelHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var userA = new User { Name = "userA", Gender = Genders.Male, Email = "userA@example.tld", YearOfBirth = 1987 };
        var userB = new User { Name = "userB", Gender = Genders.Male, Email = "userB@example.tld", YearOfBirth = 1987 };
        await InsertAsync(userA, userB);

        var request = new CreateChannelRequest(userA.Id, userB.Id);
        var handler = new CreateChannelHandler(ServiceProvider);
        var channel = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(channel);

        var context = GetDataContext();
        var inserted = context.ChatChannels.First();
        Assert.IsNotNull(inserted);

        var receivers = context.ChatChannelReceivers.ToList();
        Assert.AreEqual(2, receivers.Count);

        Assert.IsTrue(receivers.Any(r => r.UserId == userA.Id && r.ChannelId == channel.Id));
        Assert.IsTrue(receivers.Any(r => r.UserId == userB.Id && r.ChannelId == channel.Id));
    }
}