namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class GetTotalUnreadMessagesHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InitializeMessages();
    }

    [TestMethod]
    public async Task Handle_AllUnRead()
    {
        var context = GetDataContext();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var count = await Handle(mentor.Id);
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var channelMessage = context.ChatChannelMessages.ToList().First(u => u.Id == mentor.Id);
        channelMessage.ReadDate = DateTime.Now;
        context.UpdateSingleProperty(channelMessage, "ReadDate");

        var count = await Handle(mentor.Id);
        Assert.AreEqual(1, count);
    }

    private async Task<int> Handle(int userId)
    {
        var request = new GetTotalUnreadMessagesRequest(userId);
        var handler = new GetTotalUnreadMessagesHandler(ServiceProvider);
        return await handler.Handle(request, CancellationToken.None);
    }
}