// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

/// <summary>
/// A message to post in a channel
/// </summary>
public class MessagePostModel
{
    /// <summary>
    /// The content of the message
    /// </summary>
    [Required, MinLength(1)]
    public string Content { get; set; }

    /// <summary>
    /// The id of the channel
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// The id of message that is being quoted
    /// </summary>
    public int? QuotedMessageId { get; set; }

    /// <summary>
    /// The text of the quoted message (stripped)
    /// </summary>
    public string QuotedMessage { get; set; }
}