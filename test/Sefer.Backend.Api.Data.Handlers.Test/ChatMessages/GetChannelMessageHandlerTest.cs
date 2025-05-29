namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class GetChannelMessageHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var (channel, student, mentor) = await InitializePersonalChannel();
        var message = new Message { ChannelId = channel.Id, Type = MessageTypes.Text, SenderId = student.Id };
        await InsertAsync(message);

        await InsertAsync(new ChannelMessage { MessageId = message.Id, IsMarked = true, ReceiverId = student.Id });
        await InsertAsync(new ChannelMessage { MessageId = message.Id, IsMarked = true, ReceiverId = mentor.Id });
    }

    [TestMethod]
    public async Task Handle_DifferentMessageId()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);

        var channelMessage = await Handle(student.Id, 0);
        Assert.IsNull(channelMessage);
    }

    [TestMethod]
    public async Task Handle_DifferentReceiverId()
    {
        var context = GetDataContext();
        var message = context.ChatMessages.Single();

        var channelMessage = await Handle(0, message.Id);
        Assert.IsNull(channelMessage);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var message = context.ChatMessages.Single();

        var channelMessage = await Handle(student.Id, message.Id);
        Assert.IsNotNull(channelMessage);
    }

    private async Task<ChannelMessage> Handle(int userId, int messageId)
    {
        var handler = new GetChannelMessageHandler(ServiceProvider);
        var request = new GetChannelMessageRequest(userId, messageId);
        return await handler.Handle(request, CancellationToken.None);
    }
}