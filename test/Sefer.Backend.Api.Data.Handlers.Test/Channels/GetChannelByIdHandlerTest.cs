namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetChannelByIdHandlerTest : GetEntityByIdHandlerTest<GetChannelByIdRequest, GetChannelByIdHandler, Channel>
{
    protected override Task<Channel> GetEntity()
    {
        var channel = new Channel
        {
            Name = "test", Type = ChannelTypes.Personal, CreationDate = DateTime.Now
        };
        return Task.FromResult(channel);
    }
}