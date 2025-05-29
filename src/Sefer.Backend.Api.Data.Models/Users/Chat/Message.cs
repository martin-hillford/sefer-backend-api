namespace Sefer.Backend.Api.Data.Models.Users.Chat;

/// <summary>
/// Represents a message send between user
/// </summary>
/// <inheritdoc cref="Entity"/>
public class Message : Entity
{
    #region Properties

    /// <summary>
    /// The Channel this message is send into
    /// </summary>
    [InsertOnly]
    public int ChannelId { get; set; }

    /// <summary>
    /// The id of the user that has sent the message
    /// </summary>
    [InsertOnly]
    public int SenderId { get; set; }

    /// <summary>
    /// The time the sender has sent the message
    /// </summary>
    [InsertOnly]
    public DateTime SenderDate { get; set; }

    /// <summary>
    /// The Type of the message
    /// </summary>
    [InsertOnly]
    public MessageTypes Type { get; set; }

    /// <summary>
    /// Contains the reference to a referenced object, depending on the type
    /// </summary>
    [InsertOnly]
    public int? ReferenceId { get; set; }

    /// <summary>
    /// The content of the message. Depending on the type the content can be a regular string or a json string
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string ContentString { get; set; }

    /// <summary>
    /// Holds if the sender has deleted the message
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// This value holds if the message is available within the channel
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// The id of message that is being quoted
    /// </summary>
    public int? QuotedMessageId { get; set; }

    /// <summary>
    /// The text of the quoted message (stripped)
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string QuotedMessageString { get; set; }

    #endregion

    #region References

    /// <summary>
    /// Gets / sets the channel this message is posted in
    /// </summary>
    [InsertOnly]
    [ForeignKey(nameof(ChannelId))]
    public Channel Channel { get; set;}

    /// <summary>
    /// The user that has sent the message
    /// </summary>
    [InsertOnly]
    [ForeignKey(nameof(SenderId))]
    public User Sender { get ;set;}

    /// <summary>
    /// Optional the quoted messages
    /// </summary>
    [ForeignKey(nameof(QuotedMessageId))]
    public Message QuotedMessage { get; set; }

    /// <summary>
    /// When populated it contains a collection of all the metadata of the message (who read it)
    /// </summary>
    [InverseProperty(nameof(ChannelMessage.Message))]
    public ICollection<ChannelMessage> ChannelMessages { get; set;}

    #endregion
}