namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

public abstract class UsersInChannelUnitTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var userA = new User {Name = "userA", Gender = Genders.Male, Email = "userA@example.tld", YearOfBirth = 1987};
        var userB = new User {Name = "userB", Gender = Genders.Male, Email = "userB@example.tld", YearOfBirth = 1987};
        var userC = new User {Name = "userC", Gender = Genders.Male, Email = "userC@example.tld", YearOfBirth = 1987};
        var userD = new User {Name = "userD", Gender = Genders.Male, Email = "userD@example.tld", YearOfBirth = 1987};
        
        await InsertAsync(userA, userB, userC, userD);

        var channel1 = new Channel {Name = "channel1", Type = ChannelTypes.Personal};
        var channel2 = new Channel {Name = "channel2", Type = ChannelTypes.Personal};
        var channel3 = new Channel {Name = "channel3", Type = ChannelTypes.Personal};
        await InsertAsync(channel1, channel2, channel3);

        await InsertAsync(new ChannelReceiver {ChannelId = channel1.Id, UserId = userA.Id,});
        await InsertAsync(new ChannelReceiver {ChannelId = channel1.Id, UserId = userB.Id,});
        await InsertAsync(new ChannelReceiver {ChannelId = channel2.Id, UserId = userA.Id, Archived = true});
        await InsertAsync(new ChannelReceiver {ChannelId = channel2.Id, UserId = userC.Id, Archived = true});
        await InsertAsync(new ChannelReceiver {ChannelId = channel3.Id, UserId = userA.Id });
        await InsertAsync(new ChannelReceiver {ChannelId = channel3.Id, UserId = userD.Id, Deleted = true});
    }
}