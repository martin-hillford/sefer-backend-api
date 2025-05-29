namespace Sefer.Backend.Api.Chat.Test;

[TestClass]
public class ChatHubTest
{
    #region JoinChannel
    
    [TestMethod]
    public async Task JoinChannel_NoUserId()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("");
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.JoinChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task JoinChannel_NotInChannel()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.AddRequestResult<IsUserInChannelRequest,bool>(false);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.JoinChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task JoinChannel_Success()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        var users = new List<User>();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.ChatContext.Setup(c => c.GetConnectionId()).Returns("test");
        provider
            .AddRequestResult<IsUserInChannelRequest, bool>(true)
            .AddRequestResult<GetUsersInChannelRequest, List<User>>(users);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.JoinChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.AtLeastOnce);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
        provider.Groups.Verify(g => g.AddToGroupAsync("test", "channel-11", CancellationToken.None), Times.Once);
    }
    
    #endregion
    
    #region LeaveChannel
    
    [TestMethod]
    public async Task LeaveChannel_NoUserId()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("");
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.LeaveChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task LeaveChannel_NotInChannel()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.AddRequestResult<IsUserInChannelRequest,bool>(false);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.LeaveChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task LeaveChannel_Success()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        var user = new User { Id = 10, Name = "test", Active = true };
        var users = new List<User> { user };
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.ChatContext.Setup(c => c.GetConnectionId()).Returns("test");
        provider
            .AddRequestResult<IsUserInChannelRequest, bool>(true)
            .AddRequestResult<GetUsersInChannelRequest, List<User>>(users)
            .AddRequestResult<GetUserByIdRequest, User>(user);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.LeaveChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.AtLeastOnce);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
        provider.Groups.Verify(g => g.RemoveFromGroupAsync("test", "channel-11", CancellationToken.None), Times.Once);
    }
    
    #endregion
    
    #region WhoIsInChannel
    
    [TestMethod]
    public async Task WhoIsInChannel_NotInChannel()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.AddRequestResult<IsUserInChannelRequest,bool>(false);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.WhoIsInChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task WhoIsInChannel_Success()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        var user = new User { Id = 10, Name = "test", Active = true };
        var users = new List<User> { user };
        var proxy = new Mock<IClientProxy>();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.ChatContext.Setup(c => c.GetConnectionId()).Returns("test");
        provider.Clients.Setup(c => c.Group("channel-11")).Returns(proxy.Object);
        provider
            .AddRequestResult<IsUserInChannelRequest, bool>(true)
            .AddRequestResult<GetUsersInChannelRequest, List<User>>(users)
            .AddRequestResult<GetUserByIdRequest, User>(user);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.WhoIsInChannel(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.AtLeastOnce);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
        provider.Clients.Verify(g => g.Group("channel-11"), Times.AtLeastOnce());
        proxy.Verify(p => p.SendCoreAsync("onReportChannelState", It.IsAny<object[]>(), CancellationToken.None), Times.Once);
    }
    
    #endregion
    
    #region ReportChannelState
    
    [TestMethod]
    public async Task ReportChannelState_NoUserId()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("");
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.ReportChannelState(11, "online",13);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task ReportChannelState_NotInChannel()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("17");
        provider.AddRequestResult<IsUserInChannelRequest,bool>(false);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.ReportChannelState(11, "online",13);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task ReportChannelState_NoReceiver()
    {
        // Arrange
        var sendingUser = new User { Id = 17, Name = "test", Active = true };
        var provider = MockedServiceProvider.Create();
        var hub = new ChatHub(provider.Object);
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("17");
        provider
            .AddRequestResult<IsUserInChannelRequest, bool>(true)
            .AddRequestResults<GetUserByIdRequest, User?>([ sendingUser, default ]);
        
        // Act
        await hub.ReportChannelState(11, "online",13);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task ReportChannelState_Success()
    {
        // Arrange
        var sendingUser = new User { Id = 17, Name = "test", Active = true };
        var receiverUser = new User { Id = 13, Name = "test", Active = true };
        var provider = MockedServiceProvider.Create();
        var hub = new ChatHub(provider.Object);
        var proxy = new Mock<IClientProxy>();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("17");
        provider
            .AddRequestResult<IsUserInChannelRequest, bool>(true)
            .AddRequestResults<GetUserByIdRequest, User?>([ sendingUser, receiverUser ]);
        provider.Clients.Setup(c => c.User("13")).Returns(proxy.Object);
        
        // Act
        await hub.ReportChannelState(11, "online",13);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
        proxy.Verify(p => p.SendCoreAsync("onReportedChannelState", It.IsAny<object[]>(), CancellationToken.None), Times.Once);
    }
    
    #endregion

    #region Typing

    [TestMethod]
    public async Task Typing_NoUserId()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("");
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.Typing(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task Typing_NotInChannel()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("17");
        provider.AddRequestResult<IsUserInChannelRequest,bool>(false);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.Typing(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task Typing_Success()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        var proxy = new Mock<IClientProxy>();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("10");
        provider.ChatContext.Setup(c => c.GetConnectionId()).Returns("test");
        provider.Clients.Setup(c => c.Group("channel-11")).Returns(proxy.Object);
        provider.AddRequestResult<IsUserInChannelRequest, bool>(true);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.Typing(11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.AtLeastOnce);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
        provider.Clients.Verify(g => g.Group("channel-11"), Times.AtLeastOnce());
        proxy.Verify(p => p.SendCoreAsync("onTyping", It.IsAny<object[]>(), CancellationToken.None), Times.Once);
    }
    
    #endregion
    
    #region MessagesRead
    
    [TestMethod]
    public async Task MessagesRead_NoUserId()
    {
        // Arrange
        var provider = MockedServiceProvider.Create();
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("");
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.MessagesRead(19, 11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }
    
    [TestMethod]
    public async Task MessagesRead_NotMarked()
    {
        // Arrange
        var user = new User { Id = 7, Name = "test", Active = true };
        var provider = MockedServiceProvider.Create();
        provider.AddRequestResult<MarkMessageAsReadRequest, bool>(false);
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("7");
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.MessagesRead(19, 11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.Never);
    }    
    
    [TestMethod]
    public async Task MessagesRead_Success()
    {
        // Arrange
        var proxy = new Mock<IClientProxy>();
        var user = new User { Id = 7, Name = "test", Active = true };
        var provider = MockedServiceProvider.Create();
        provider.AddRequestResult<MarkMessageAsReadRequest, bool>(true);
        provider.ChatContext.Setup(c => c.GetUserIdentifier()).Returns("7");
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        provider.Clients.Setup(c => c.Group("channel-19")).Returns(proxy.Object);
        var hub = new ChatHub(provider.Object);
        
        // Act
        await hub.MessagesRead(19, 11);
        
        // Assert
        provider.ChatContext.Verify(c => c.GetUserIdentifier(), Times.Once);
        provider.ChatContext.Verify(c => c.GetClients(), Times.AtLeastOnce);
    }   
    
    #endregion
}