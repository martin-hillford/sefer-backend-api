namespace Sefer.Backend.Api.Chat;

/// <summary>
/// This interface is used to make the chat hub more testable.
/// It must be a service that should not be provided by the
/// service provider, only in test situations
/// </summary>
public interface IChatHubContext
{
    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public string GetUserIdentifier();

    /// <summary>
    /// Gets the connection ID.
    /// </summary>
    public string GetConnectionId();

    /// <summary>
    /// Gets the group manager.
    /// </summary>
    public IGroupManager GetGroups();

    /// <summary>
    /// Gets an object that can be used to invoke methods on the clients connected to this hub.
    /// </summary>
    public IHubCallerClients GetClients();
}

