namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostMentorChangeChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_StudentNull()
    {
        var messages = await Handle(null, null, null, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NewMentorNull()
    {
        var student = new User();
        var messages = await Handle(student, null, null, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_StudentIsMentor()
    {
        var student = new User { Role = UserRoles.Mentor };
        var newMentor = new User();
        var messages = await Handle(student, newMentor, null, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_NewMentorNotMentor()
    {
        var student = new User { Role = UserRoles.Student };
        var newMentor = new User { Role = UserRoles.Student };
        var messages = await Handle(student, newMentor, null, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_OldMentorIsNoMentor()
    {
        var student = new User { Role = UserRoles.Student };
        var newMentor = new User { Role = UserRoles.Mentor };
        var oldMentor = new User { Role = UserRoles.Student };
        var messages = await Handle(student, newMentor, oldMentor, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_OldMentorNull()
    {
        var student = new User { Role = UserRoles.Student };
        var newMentor = new User { Role = UserRoles.Mentor };
        var messages = await Handle(student, newMentor, null, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_HandleNoNewChannel()
    {
        var student = new User { Role = UserRoles.Student };
        var newMentor = new User { Role = UserRoles.Mentor };
        var oldMentor = new User { Role = UserRoles.Mentor };
        var messages = await Handle(student, newMentor, oldMentor, null, null);
        messages.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Handle_OldMentorChannelNull()
    {
        var (newChannel, student, newMentor) = await InitializePersonalChannel();
        var (oldChannel, oldMentor) = await InitializeOldMentor(false);
        var messages = await Handle(student, newMentor, oldMentor, newChannel, oldChannel);
        messages.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle()
    {
        var (newChannel, student, newMentor) = await InitializePersonalChannel();
        var (oldChannel, oldMentor) = await InitializeOldMentor(true);
        var message = await Handle(student, newMentor, oldMentor, newChannel, oldChannel);
        message.Count.Should().Be(2);
    }

    private async Task<(Channel? channel, User mentor)> InitializeOldMentor(bool createChannel)
    {
        var context = GetDataContext();
        var mentor = new User { Role = UserRoles.Mentor, Name = "Mentor2", Gender = Genders.Male, Email = "mentor2@example.tld", YearOfBirth = 1987 };
        await InsertAsync(mentor);

        if (!createChannel) return (null, mentor);

        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var channel = new Channel { Name = "personal", Type = ChannelTypes.Personal };
        await InsertAsync(channel);

        var studentRev = new ChannelReceiver { ChannelId = channel.Id, UserId = student.Id, HasPostRights = true };
        var mentorRev = new ChannelReceiver { ChannelId = channel.Id, UserId = mentor.Id, HasPostRights = true };
        await InsertAsync(studentRev, mentorRev);

        return (channel, mentor);
    }

    private async Task<List<Message>> Handle(User? student, User? newMentor, User? oldMentor, Channel? newChannel, Channel? oldChannel)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResults<GetUserByIdRequest, User>(student, newMentor, oldMentor);
        provider.AddRequestResults<GetPersonalChannelRequest, Channel>(newChannel, oldChannel);
        provider.AddRequestResult<IsUserInChannelRequest, bool>(true);
        provider.AddRequestResults<GetChannelByIdRequest, Channel>(newChannel, oldChannel);
        var request = new PostMentorChangeChatMessageRequest
        {
            Student = student?.Id ?? -1,
            NewMentor = newMentor?.Id ?? -1,
            OldMentor = oldMentor?.Id ?? -1,
            CourseName = "course"
        };
        var handler = new PostMentorChangeChatMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}

