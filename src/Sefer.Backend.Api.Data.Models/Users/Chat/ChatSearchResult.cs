namespace Sefer.Backend.Api.Data.Models.Users.Chat;

/// <summary>
/// This model represents the result of search for message in the chat
/// </summary>
public class ChatSearchResult
{
    /// <summary>
    /// id of the channel the message is in
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// The id of the message itself
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// The content of the message itself
    /// </summary>
    public string Content { get; set; }
}