namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

public abstract class UnreadMessageUnitTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var userA = new User {Name = "userA", Gender = Genders.Male, Email = "userA@example.tld", YearOfBirth = 1987};
        var userB = new User {Name = "userB", Gender = Genders.Male, Email = "userB@example.tld", YearOfBirth = 1987};
        var userC = new User {Name = "userC", Gender = Genders.Male, Email = "userC@example.tld", YearOfBirth = 1987};
        await InsertAsync(userA, userB, userC);
        
        var channel1 = new Channel {Name = "channel1", Type = ChannelTypes.Personal};
        var channel2 = new Channel {Name = "channel2", Type = ChannelTypes.Personal};
        await InsertAsync(channel1, channel2);
        
        await InsertAsync(new ChannelReceiver {ChannelId = channel1.Id, UserId = userA.Id,});
        await InsertAsync(new ChannelReceiver {ChannelId = channel1.Id, UserId = userB.Id,});
        
        await InsertAsync(new ChannelReceiver {ChannelId = channel2.Id, UserId = userA.Id });
        await InsertAsync(new ChannelReceiver {ChannelId = channel2.Id, UserId = userC.Id });
        
        var message1 = new Message { ChannelId = channel1.Id, ContentString = "1", SenderDate = DateTime.Now, SenderId = userB.Id };
        var message2 = new Message { ChannelId = channel1.Id, ContentString = "2", SenderDate = DateTime.Now, SenderId = userB.Id };
        var message3 = new Message { ChannelId = channel2.Id, ContentString = "3", SenderDate = DateTime.Now, SenderId = userC.Id };
        await InsertAsync(message1, message2, message3);

        await InsertAsync(new ChannelMessage {MessageId = message1.Id, ReceiverId = userA.Id});
        await InsertAsync(new ChannelMessage {MessageId = message1.Id, ReceiverId = userB.Id});

        await InsertAsync(new ChannelMessage {MessageId = message2.Id, ReceiverId = userA.Id});
        await InsertAsync(new ChannelMessage {MessageId = message2.Id, ReceiverId = userB.Id});
        
        await InsertAsync(new ChannelMessage {MessageId = message3.Id, ReceiverId = userA.Id});
        await InsertAsync(new ChannelMessage {MessageId = message3.Id, ReceiverId = userC.Id});
    }
}