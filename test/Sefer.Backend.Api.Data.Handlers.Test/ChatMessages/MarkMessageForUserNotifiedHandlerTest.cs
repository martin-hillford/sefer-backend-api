namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class MarkMessageForUserNotifiedHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider(new Exception());
        var request = new MarkMessageForUserNotifiedRequest(19, 13);
        var handler = new MarkMessageForUserNotifiedHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle()
    {
        var result = await Handle(null);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task Handle_DifferentMessage()
    {
        var result = await Handle(13);
        result.Should().BeFalse();
    }

    private async Task<bool> Handle(int? messageToMark)
    {
        var (channel, student, mentor) = await InitializePersonalChannel();
        var message = new Message { Type = MessageTypes.Text, ChannelId = channel.Id, SenderId = student.Id, ContentString = "test" };
        await InsertAsync(message);

        var msgStudent = new ChannelMessage { MessageId = message.Id, ReceiverId = student.Id };
        var msgMentor = new ChannelMessage { MessageId = message.Id, ReceiverId = mentor.Id };
        await InsertAsync(msgStudent, msgMentor);

        var provider = GetServiceProvider();
        var request = new MarkMessageForUserNotifiedRequest(messageToMark ?? message.Id, student.Id);
        var handler = new MarkMessageForUserNotifiedHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}