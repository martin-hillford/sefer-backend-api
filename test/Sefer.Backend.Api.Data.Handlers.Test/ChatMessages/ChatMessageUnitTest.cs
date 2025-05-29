namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

public abstract class ChatMessageUnitTest : HandlerUnitTest
{
    protected async Task<(Channel channel, User student, User mentor)> InitializePersonalChannel()
    {
        var student = new User { Role = UserRoles.Student, Name = "Student", Gender = Genders.Male, Email = "student@example.tld", YearOfBirth = 1987 };
        var mentor = new User { Role = UserRoles.Mentor, Name = "Mentor", Gender = Genders.Male, Email = "mentor@example.tld", YearOfBirth = 1987 };
        await InsertAsync(student, mentor);

        var channel = new Channel { Name = "personal", Type = ChannelTypes.Personal };
        await InsertAsync(channel);

        var studentRev = new ChannelReceiver { ChannelId = channel.Id, UserId = student.Id, HasPostRights = true };
        var mentorRev = new ChannelReceiver { ChannelId = channel.Id, UserId = mentor.Id, HasPostRights = true };
        await InsertAsync(studentRev, mentorRev);

        return (channel, student, mentor);
    }

    protected async Task InitializeMessages()
    {
        var (channel, student, mentor) = await InitializePersonalChannel();
        var messageA = new Message { ChannelId = channel.Id, Type = MessageTypes.Text, IsAvailable = true, SenderId = student.Id, ContentString = "A", SenderDate = DateTime.UtcNow.AddMinutes(-1) };
        var messageB = new Message { ChannelId = channel.Id, Type = MessageTypes.Text, IsAvailable = true, SenderId = student.Id, ContentString = "B", SenderDate = DateTime.UtcNow.AddDays(-1) };
        await InsertAsync(messageA, messageB);

        await InsertAsync(new ChannelMessage { MessageId = messageA.Id, IsMarked = true, ReceiverId = student.Id });
        await InsertAsync(new ChannelMessage { MessageId = messageA.Id, IsMarked = true, ReceiverId = mentor.Id });
        await InsertAsync(new ChannelMessage { MessageId = messageB.Id, IsMarked = true, ReceiverId = student.Id });
        await InsertAsync(new ChannelMessage { MessageId = messageB.Id, IsMarked = true, ReceiverId = mentor.Id });
    }
}