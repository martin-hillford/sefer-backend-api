namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class GetUnNotifiedMessageOfUserHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeMessages();

    [TestMethod]
    public async Task Handle_OnlyUnNotified()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var message = context.ChatChannelMessages.First(m => m.ReceiverId == student.Id);
        message.IsNotified = true;
        context.UpdateSingleProperty(message, "IsNotified");

        var messages = await Execute();
        messages.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle_WithDelay()
    {
        var messages = await Execute(1000);
        messages.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle()
    {
        var messages = await Execute();
        messages.Count.Should().Be(2);
    }

    private async Task<List<Message>> Execute(int delay = -100)
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var request = new GetUnNotifiedMessageOfUserRequest(student.Id, delay);
        var provider = GetServiceProvider();
        var handler = new GetUnNotifiedMessageOfUserHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}