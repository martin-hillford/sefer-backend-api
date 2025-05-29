// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Chat.Models;

/// <summary>
/// This represents the incoming message
/// </summary>
public class SendingMessage
{
    /// <summary>
    /// The text for the message
    /// </summary>
    public string Content { get; set;}

    /// <summary>
    /// The channelId into which the message is sent
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// Some tempId used by the sending client
    /// </summary>
    public string TempId { get; set; }

    /// <summary>
    /// The id of message that is being quoted
    /// </summary>
    public int? QuotedMessageId { get; set; }

    /// <summary>
    /// The text of the quoted message (stripped)
    /// </summary>
    public string QuotedMessage { get; set; }
}