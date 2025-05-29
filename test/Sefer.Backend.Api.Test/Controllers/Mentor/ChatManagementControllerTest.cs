using Sefer.Backend.Api.Controllers.Mentor;
using Sefer.Backend.Api.Models.Mentor;
using Sefer.Backend.Api.Notifications.WebSocket;
using Sefer.Backend.Api.Views.Shared.Users.Chat;

namespace Sefer.Backend.Api.Test.Controllers.Mentor;

[TestClass]
public class ChatManagementControllerTest : AbstractControllerTest
{
    #region CreateChannel
    
    [TestMethod]
    public async Task CreateChannel_NoMentor()
    {
        // Arrange - Set up the provider
        var user = new User { Role = UserRoles.Student, Id = 13 };
        var provider = GetServiceProvider(user);

        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var channel = new ChannelPostModel { Name = "name" };
        
        // Act - Create the channel
        var result = await controller.CreateChannel(channel);

        // Assert - Channel should not be created
        result.Should().NotBeNull();
        result.Should().BeOfType<ForbidResult>();
    }
    
    [TestMethod]
    public async Task CreateChannel_NameAlreadyExists()
    {
        // Arrange - Set up the provider
        var user = new User { Role = UserRoles.Mentor, Id = 13 };
        var provider = GetServiceProvider(user);
        var channels = new List<Channel> { new() { Name = "name", Type = ChannelTypes.Private} };
        provider.AddRequestResult<GetChannelsRequest, List<Channel>>(channels);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var channel = new ChannelPostModel { Name = "name" };
        
        // Act - Create the channel
        var result = await controller.CreateChannel(channel);
        
        // Assert - Channel should not be created
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [TestMethod]
    public async Task CreateChannel_Success()
    {
        // Arrange - Set up the provider
        var channel = new Channel { Name = "name", Type = ChannelTypes.Private };
        var user = new User { Role = UserRoles.Mentor, Id = 13 };
        var provider = GetServiceProvider(user)
            .AddService(new Mock<IWebSocketProvider>())
            .AddRequestResult<GetChannelsRequest, List<Channel>>([])
            .AddRequestResult<CreateGroupChannelRequest, Channel>(channel)
            .AddRequestResult<AddChannelReceiverRequest, Channel>(channel);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelPostModel { Name = "name" };
        
        // Act - Create the channel
        var result = await controller.CreateChannel(body);

        // Assert - Channel should be created
        result.Should().NotBeNull();
        result.Should().BeOfType<JsonResult>();
        var view = ((result as JsonResult)?.Value as ChannelView);
        view.Should().NotBeNull();
        if (view == null) return;
        view.Type.Should().Be(ChannelTypes.Private);
        view.Name.Should().Be("name");
    }
    
    #endregion
    
    #region AddStudentToChannel
    
    [TestMethod]
    public async Task AddStudentToChannel_Invalid()
    {
        // Arrange - Set up the provider
        var provider = AddStudentToChannel_GetServiceProvider(false, null);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ]};
        
        // Act - Create the channel
        var result = await controller.AddStudentToChannel(body);
        
        // Assert - Channel should be created
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();
    }

    [TestMethod]
    public async Task AddStudentToChannel_FailedToAdd()
    {
        // Arrange - Set up the provider
        var provider = AddStudentToChannel_GetServiceProvider(true, null);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ]};
        
        // Act - Create the channel
        var result = await controller.AddStudentToChannel(body);
        
        // Assert - Channel should be created
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [TestMethod]
    public async Task AddStudentToChannel_Success()
    {
        // Arrange - Set up the provider
        var channel = new Channel { Name = "name" };
        var provider = AddStudentToChannel_GetServiceProvider(true, channel);

        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ]};
        
        // Act - Create the channel
        var result = await controller.AddStudentToChannel(body);
        
        // Assert - Student should be added
        result.Should().NotBeNull();
        result.Should().BeOfType<JsonResult>();
    }

    private static MockedServiceProvider AddStudentToChannel_GetServiceProvider(bool isStudentOfMentorRequest, Channel channel)
    {
        var mentor = new User { Role = UserRoles.Mentor, Id = 13 };
        return GetServiceProvider(mentor)
            .AddService(new Mock<IWebSocketProvider>())
            .AddRequestResult<IsStudentOfMentorRequest, bool>(isStudentOfMentorRequest)
            .AddRequestResult<GetUsersInChannelRequest, List<User>>([mentor])
            .AddRequestResult<AddChannelReceiverRequest, Channel>(channel);
    }
    
    #endregion
    
    #region RemoveStudentFromChannel
    
    [TestMethod]
    public async Task RemoveStudentFromChannel_Invalid()
    {
        // Arrange - Set up the provider
        var provider = RemoveStudentFromChannel_GetServiceProvider(false, null);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ] };
        
        // Act - Create the channel
        var result = await controller.RemoveStudentFromChannel(body);
        
        // Assert - Channel should be created
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [TestMethod]
    public async Task RemoveStudentFromChannel_FailedToAdd()
    {
        // Arrange - Set up the provider
        var provider = RemoveStudentFromChannel_GetServiceProvider(true, null);
        
        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ]};
        
        // Act - Create the channel
        var result = await controller.RemoveStudentFromChannel(body);
        
        // Assert - Channel should be created
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [TestMethod]
    public async Task RemoveStudentFromChannel_Success()
    {
        // Arrange - Set up the provider
        var channel = new Channel { Name = "name" };
        var provider = RemoveStudentFromChannel_GetServiceProvider(true, channel);

        // Arrange - Create controller and post model
        var controller = new ChatManagementController(provider.Object);
        var body = new ChannelStudentPostModel { ChannelId = 7, Students = [ 11 ]};
        
        // Act - Create the channel
        var result = await controller.RemoveStudentFromChannel(body);
        
        // Assert - Student should be removed
        result.Should().NotBeNull();
        result.Should().BeOfType<JsonResult>();
    }
    
    private static MockedServiceProvider RemoveStudentFromChannel_GetServiceProvider(bool isStudentOfMentorRequest, Channel channel)
    {
        var mentor = new User { Role = UserRoles.Mentor, Id = 13 };
        var student = new User { Role = UserRoles.Student, Id = 11 };
        return GetServiceProvider(mentor)
            .AddService(new Mock<IWebSocketProvider>())
            .AddRequestResult<IsStudentOfMentorRequest, bool>(isStudentOfMentorRequest)
            .AddRequestResult<GetUsersInChannelRequest, List<User>>([mentor, student])
            .AddRequestResult<RemoveChannelReceiverRequest, Channel>(channel);
    }
    
    #endregion
}