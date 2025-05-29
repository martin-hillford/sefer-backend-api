namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class CreateGroupChannelHandlerTest  : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var mentor = new User { Name = "Mentor", Gender = Genders.Male, Email = "mentor@example.tld", YearOfBirth = 1987, Role = UserRoles.Mentor};
        await InsertAsync(mentor);
        
        var request = new CreateGroupChannelRequest(mentor.Id, "channel");
        var handler = new CreateGroupChannelHandler(ServiceProvider);
        var channel = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(channel);

        var context = GetDataContext();
        var inserted = context.ChatChannels.First();
        Assert.IsNotNull(inserted);

        var receivers = context.ChatChannelReceivers.ToList();
        Assert.AreEqual(1, receivers.Count);
        Assert.IsTrue(receivers.Any(r => r.UserId == mentor.Id && r.ChannelId == channel.Id));
    }
    
    [TestMethod]
    public async Task Handle_NotMentor_Null()
    {
        var mentor = new User { Name = "Student", Gender = Genders.Male, Email = "student@example.tld", YearOfBirth = 1987, Role = UserRoles.Student};
        await InsertAsync(mentor);
        
        var request = new CreateGroupChannelRequest(mentor.Id, "channel");
        var handler = new CreateGroupChannelHandler(ServiceProvider);
        var channel = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(channel);
    }
}