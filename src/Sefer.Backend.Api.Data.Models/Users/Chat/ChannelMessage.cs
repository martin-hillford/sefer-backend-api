namespace Sefer.Backend.Api.Data.Models.Users.Chat;

/// <summary>
/// When a sender send a message into a channel for each of the receivers it is recorded
/// if the receiver has read the message (metadata)
/// </summary>
/// <inheritdoc cref="Entity"/>
public class ChannelMessage : Entity
{
    /// <summary>
    /// Gets / sets the receiver of the message into the channel
    /// </summary>
    [InsertOnly]
    public int ReceiverId { get; set; }

    /// <summary>
    /// The message this metadata is about
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// The date the receiver has read the message
    /// </summary>
    public DateTime? ReadDate { get; set; }

    /// <summary>
    /// Holds if the receiver has pinned / marked the message
    /// </summary>
    public bool IsMarked { get; set; }

    /// <summary>
    /// Holds if the receiver has been notified he has new message
    /// </summary>
    public bool IsNotified { get; set; }

    /// <summary>
    /// Gets / sets the receiver of the message into the channel
    /// </summary>
    [InsertOnly, ForeignKey(nameof(ReceiverId))]
    public User Receiver { get; set; }

    /// <summary>
    /// The message this metadata is about
    /// </summary>
    [InsertOnly, ForeignKey(nameof(MessageId))]
    public Message Message { get; set; }
}