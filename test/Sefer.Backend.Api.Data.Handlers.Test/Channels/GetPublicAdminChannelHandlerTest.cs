namespace Sefer.Backend.Api.Data.Handlers.Test.Channels;

[TestClass]
public class GetPublicAdminChannelHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoUser()
    {
        var provider = GetServiceProvider();
        var channel = await Handle(19, provider);
        Assert.IsNull(channel);
    }
    
    [TestMethod]
    public async Task Handle_UserIsAdmin()
    {
        var user = new User { Role = UserRoles.Admin };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var channel = await Handle(19, provider);
        Assert.IsNull(channel);
    }
    
    [TestMethod]
    public async Task Handle_AdminChannelExist()
    {
        var user = new User { Name = "userA", Role = UserRoles.Student, Gender = Genders.Male, Email = "uA@e.tld", YearOfBirth = 1987 };
        var admin = new User { Name = "userB", Role = UserRoles.Admin, Gender = Genders.Male, Email = "aB@e.tld", YearOfBirth = 1987 };
        await InsertAsync(user, admin);
        await InsertAsync(new AdminSettings {AdminId = admin.Id, IsPublicAdmin = true});
        
        var channel = new Channel {Name = "channel1", Type = ChannelTypes.Personal};
        await InsertAsync(channel);
        
        await InsertAsync(new ChannelReceiver {ChannelId = channel.Id, UserId = user.Id,});
        await InsertAsync(new ChannelReceiver {ChannelId = channel.Id, UserId = admin.Id,});
        
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var result = await Handle(user.Id, provider);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(channel.Name, result.Name);
    }
    
    [TestMethod]
    public async Task Handle_NoPublicAdmin()
    {
        
        var user = new User { Role = UserRoles.Student };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var channel = await Handle(19, provider);
        Assert.IsNull(channel);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var user = new User { Name = "userA", Role = UserRoles.Student, Gender = Genders.Male, Email = "uA@e.tld", YearOfBirth = 1987 };
        var admin = new User { Name = "userB", Role = UserRoles.Admin, Gender = Genders.Male, Email = "aB@e.tld", YearOfBirth = 1987 };
        await InsertAsync(user, admin);
        await InsertAsync(new AdminSettings {AdminId = admin.Id, IsPublicAdmin = true});
        
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        provider.AddRequestResult<GetPublicAdminRequest, User>(admin);
        
        await Handle(user.Id, provider);
        
        provider.Mediator.Verify(m => m.Send(It.IsAny<CreateChannelRequest>(), CancellationToken.None));
    }

    private async Task<Channel> Handle(int user, MockedServiceProvider? provider = null)
    {
        var request = new GetPublicAdminChannelRequest(user);
        var handler = new GetPublicAdminChannelHandler(provider?.Object ?? ServiceProvider);
        return await handler.Handle(request, CancellationToken.None);
    }
}