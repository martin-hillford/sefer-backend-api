namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class GetUnreadMessageOfUserHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeMessages();

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var messages = await Handle(mentor.Id);
        Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public async Task Handle_SomeRead()
    {
        var context = GetDataContext();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var message = context.ChatChannelMessages.ToList().First(m => m.ReceiverId == mentor.Id);
        message.ReadDate = DateTime.UtcNow;
        context.UpdateSingleProperty(message, nameof(message.ReadDate));
        var messages = await Handle(mentor.Id);
        Assert.AreEqual(1, messages.Count);
    }

    private async Task<List<Message>> Handle(int userId)
    {
        var handle = new GetUnreadMessageOfUserHandler(ServiceProvider);
        var request = new GetUnreadMessageOfUserRequest(userId);
        return await handle.Handle(request, CancellationToken.None);
    }
}