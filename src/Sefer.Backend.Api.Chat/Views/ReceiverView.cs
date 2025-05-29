// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Chat.Views;

/// <summary>
/// Holds information about the users that has read or not the message
/// </summary>
public class ReceiverView
{
    /// <summary>
    /// Holds if the user has read the message
    /// </summary>
    public bool HasRead => ReadDate != null;

    /// <summary>
    /// Holds when the user has read the message
    /// </summary>
    public readonly DateTime? ReadDate;

    /// <summary>
    /// Holds the name of user
    /// </summary>
    public readonly string UserName;

    /// <summary>
    /// The id of the user that has received the message
    /// </summary>
    public readonly int UserId;

    /// <summary>
    /// Create a new ReceiverView
    /// </summary>
    /// <param name="channelMessage"></param>
    public ReceiverView(ChannelMessage channelMessage)
    {
        ReadDate = channelMessage.ReadDate;
        UserName = channelMessage.Receiver.Name;
        UserId = channelMessage.Receiver.Id;
    }
}