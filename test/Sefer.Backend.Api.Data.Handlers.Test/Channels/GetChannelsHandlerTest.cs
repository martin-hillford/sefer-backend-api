namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetChannelsHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var userA = new User { Name = "userA", Gender = Genders.Male, Email = "userA@example.tld", YearOfBirth = 1987 };
        var userB = new User { Name = "userB", Gender = Genders.Male, Email = "userB@example.tld", YearOfBirth = 1987 };
        var userC = new User { Name = "userC", Gender = Genders.Male, Email = "userC@example.tld", YearOfBirth = 1987 };
        var userD = new User { Name = "userD", Gender = Genders.Male, Email = "userD@example.tld", YearOfBirth = 1987 };
        var userE = new User { Name = "userE", Gender = Genders.Male, Email = "userE@example.tld", YearOfBirth = 1987 };

        await InsertAsync(userA, userB, userC, userD, userE);

        var channel1 = new Channel { Name = "channel1", Type = ChannelTypes.Personal };
        var channel2 = new Channel { Name = "channel2", Type = ChannelTypes.Personal };
        var channel3 = new Channel { Name = "channel3", Type = ChannelTypes.Personal };
        var channel4 = new Channel { Name = "channel4", Type = ChannelTypes.Personal };
        await InsertAsync(channel1, channel2, channel3, channel4);

        await InsertAsync(new ChannelReceiver { ChannelId = channel1.Id, UserId = userA.Id, });
        await InsertAsync(new ChannelReceiver { ChannelId = channel1.Id, UserId = userB.Id, });
        await InsertAsync(new ChannelReceiver { ChannelId = channel2.Id, UserId = userA.Id, Archived = true });
        await InsertAsync(new ChannelReceiver { ChannelId = channel2.Id, UserId = userC.Id, Archived = true });
        await InsertAsync(new ChannelReceiver { ChannelId = channel3.Id, UserId = userA.Id, Deleted = true });
        await InsertAsync(new ChannelReceiver { ChannelId = channel3.Id, UserId = userD.Id, Deleted = true });
        await InsertAsync(new ChannelReceiver { ChannelId = channel4.Id, UserId = userA.Id, });
        await InsertAsync(new ChannelReceiver { ChannelId = channel4.Id, UserId = userE.Id, });

        await InsertAsync(new Message { ChannelId = channel1.Id, ContentString = "1", SenderDate = DateTime.Now, SenderId = userA.Id });
        await InsertAsync(new Message { ChannelId = channel1.Id, ContentString = "2", SenderDate = DateTime.Now.AddDays(1), SenderId = userA.Id });
        await InsertAsync(new Message { ChannelId = channel1.Id, ContentString = "3", SenderDate = DateTime.Now.AddDays(-1), SenderId = userA.Id });

        await InsertAsync(new Message { ChannelId = channel4.Id, ContentString = "4", SenderDate = DateTime.Now, SenderId = userA.Id });
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var user = context.Users.Single(u => u.Name == "userA");

        var request = new GetChannelsRequest(user.Id);
        var handler = new GetChannelsHandler(ServiceProvider);
        var channels = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(2, channels.Count);
        var channel = channels.First();
        Assert.AreEqual("channel1", channel.Name);

        Assert.AreEqual(2, channel.Receivers.Count);
        Assert.IsTrue(channel.Receivers.Any(r => r.User.Name == "userA"));
        Assert.IsTrue(channel.Receivers.Any(r => r.User.Name == "userB"));
    }
}