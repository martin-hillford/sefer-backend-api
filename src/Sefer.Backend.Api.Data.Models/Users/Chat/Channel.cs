namespace Sefer.Backend.Api.Data.Models.Users.Chat;

/// <summary>
/// A Channel is used to send message into (it's comparable with a postbox)
/// </summary>
/// <inheritdoc cref="Entity"/>
public class Channel : Entity
{
    /// <summary>
    /// The DateTime this channel was created
    /// </summary>
    [InsertOnly]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The type of the channel
    /// </summary>
    [InsertOnly]
    public ChannelTypes Type { get; set; }

    /// <summary>
    /// The name of channel (when it is not a personal channel)
    /// </summary>
    [MaxLength(1023)]
    public string Name { get; set; }

    /// <summary>
    /// When populated it contains a collection of all the receivers in this channel
    /// </summary>
    [InverseProperty(nameof(ChannelReceiver.Channel))]
    public ICollection<ChannelReceiver> Receivers { get; set; }

    /// <summary>
    /// When populated it contains a collection of all the messages in this channel
    /// </summary>
    [InverseProperty(nameof(Message.Channel))]
    public ICollection<Message> Messages { get; set; }
}