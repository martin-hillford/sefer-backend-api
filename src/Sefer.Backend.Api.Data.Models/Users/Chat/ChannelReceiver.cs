namespace Sefer.Backend.Api.Data.Models.Users.Chat;

/// <summary>
/// A channel receiver is all about which user is listening in which channel.
/// Basically it describes the view of the user on the channel
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class ChannelReceiver : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// Gets / sets the userId this subscription is about
    /// </summary>
    [InsertOnly]
    public int UserId { get; set; }

    /// <summary>
    /// Gets / sets the channel this subscription is about
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// Gets / sets if the user has post rights in the channel
    /// </summary>
    public bool HasPostRights { get; set; }

    /// <summary>
    /// Gets / sets if the user has archived this channel
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Gets / sets if the user has deleted the subscription on the channel
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets / sets the channel this subscription is about
    /// </summary>
    [ForeignKey(nameof(ChannelId))]
    public Channel Channel { get; set; }

    /// <summary>
    /// Gets / sets the user that is subscribed to this channel
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    #endregion
}