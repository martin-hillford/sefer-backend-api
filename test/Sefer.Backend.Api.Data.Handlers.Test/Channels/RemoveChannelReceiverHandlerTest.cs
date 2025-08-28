namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class RemoveChannelReceiverHandlerTest  : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoChannel()
    {
        // Arrange
        var request = new RemoveChannelReceiverRequest(1, [2]);
        var handler = new RemoveChannelReceiverHandler(ServiceProvider);
        
        // Act
        var channel = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.IsNull(channel);
    }

    [TestMethod]
    public async Task Handle_NoExistingReceiver()
    {
        // Arrange - Prepare Data
        await InsertAsync(new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987, Id = 11});
        await InsertAsync(new Channel { Id = 7, Name = "test", Type = ChannelTypes.Private });
        
        // Arrange - Create handler
        var request = new RemoveChannelReceiverRequest(7, [11]);
        var handler = new RemoveChannelReceiverHandler(ServiceProvider);
        
        // Act
        var channel = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        channel.Should().NotBeNull();
        channel.Id.Should().Be(7);
        channel.Receivers.Should().HaveCount(0);
    }
    
    [TestMethod]
    public async Task Handle_Success()
    {
        // Arrange - Prepare Data
        await InsertAsync(new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987, Id = 11});
        await InsertAsync(new Channel { Id = 7, Name = "test", Type = ChannelTypes.Private });
        await InsertAsync(new ChannelReceiver { ChannelId = 7, UserId = 11 });
        
        // Arrange - Create handler
        var request = new RemoveChannelReceiverRequest(7, [11]);
        var handler = new RemoveChannelReceiverHandler(ServiceProvider);
        
        // Act
        var channel = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        channel.Should().NotBeNull();
        channel.Id.Should().Be(7);
        channel.Receivers.Should().HaveCount(0);
    }
}