namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class LoadMessageReferencesHandlerTest : ChatMessageUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InitializeMessages();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var messages = context.ChatMessages.Include(message => message.Channel)
            .Include(message => message.ChannelMessages).ToList();
        var request = new LoadMessageReferencesRequest(messages);
        var handler = new LoadMessageReferencesHandler(ServiceProvider);

        await handler.Handle(request, CancellationToken.None);

        foreach (var message in messages)
        {
            Assert.IsNotNull(message.Channel);
            Assert.IsNotNull(message.ChannelMessages);
        }
    }
}