namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetPersonalChannelHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var userA = new User { Name = "userA", Gender = Genders.Male, Email = "userA@example.tld", YearOfBirth = 1987 };
        var userB = new User { Name = "userB", Gender = Genders.Male, Email = "userB@example.tld", YearOfBirth = 1987 };
        var userC = new User { Name = "userC", Gender = Genders.Male, Email = "userC@example.tld", YearOfBirth = 1987 };

        await InsertAsync(userA, userB, userC);

        var channel1 = new Channel { Name = "channel1", Type = ChannelTypes.Personal };
        await InsertAsync(channel1);

        await InsertAsync(new ChannelReceiver { ChannelId = channel1.Id, UserId = userA.Id, });
        await InsertAsync(new ChannelReceiver { ChannelId = channel1.Id, UserId = userB.Id, });
    }

    [TestMethod]
    public async Task Handle_Existing()
    {
        var context = GetDataContext();
        var userA = context.Users.Single(u => u.Name == "userA");
        var userB = context.Users.Single(u => u.Name == "userB");

        var request = new GetPersonalChannelRequest(userA.Id, userB.Id);
        var handler = new GetPersonalChannelHandler(ServiceProvider);
        var channel = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(channel);
        Assert.AreEqual("channel1", channel.Name);
    }

    [TestMethod]
    public async Task Handle_ChannelNull()
    {
        var context = GetDataContext();
        var userA = context.Users.Single(u => u.Name == "userA");
        var userC = context.Users.Single(u => u.Name == "userC");
        var provider = GetServiceProvider();

        var request = new GetPersonalChannelRequest(userA.Id, userC.Id);
        var handler = new GetPersonalChannelHandler(provider.Object);
        var channel = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(channel);
    }
}