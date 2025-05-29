namespace Sefer.Backend.Api.Chat;

public class ChatHubContext(Hub hub) : IChatHubContext
{
    public string GetUserIdentifier()
    {
        return hub.Context.UserIdentifier;
    }

    public string GetConnectionId()
    {
        return hub.Context.ConnectionId;
    }

    public IGroupManager GetGroups()
    {
        return hub.Groups;
    }

    public IHubCallerClients GetClients()
    {
        return hub.Clients;
    }
}