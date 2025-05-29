namespace Sefer.Backend.Api.Chat;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class ChatHub(IServiceProvider serviceProvider) : Hub
{
    public async Task JoinChannel(int channelId)
    {
        var implementation = GetImplementation();
        await implementation.JoinChannel(channelId);
    }

    public async Task LeaveChannel(int channelId)
    {
        var implementation = GetImplementation();
        await implementation.LeaveChannel(channelId);
    }

    public async Task WhoIsInChannel(int channelId)
    {
        var implementation = GetImplementation();
        await implementation.WhoIsInChannel(channelId);
    }

    public async Task ReportChannelState(int channelId, string state, int receiverId)
    {
        var implementation = GetImplementation();
        await implementation.ReportChannelState(channelId, state, receiverId);
    }

    public async Task SendMessage(SendingMessage message)
    {
        var implementation = GetImplementation();
        await implementation.SendMessage(message);
    }

    public async Task MessagesRead(int channelId, int messageId)
    {
        var implementation = GetImplementation();
        await implementation.MessagesRead(channelId, messageId);   
    }

    public async Task Typing(int channelId)
    {
        var implementation = GetImplementation();
        await implementation.Typing(channelId);     
    }
    
    private ChatHubImpl GetImplementation()
    {
        var notificationService = serviceProvider.GetService<INotificationService>();
        var mediator = serviceProvider.GetService<IMediator>();
        var context = serviceProvider.GetService<IChatHubContext>() ?? new ChatHubContext(this);
        return new ChatHubImpl(mediator,context, notificationService);
    }
}