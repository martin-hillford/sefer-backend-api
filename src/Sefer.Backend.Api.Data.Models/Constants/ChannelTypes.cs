namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Defines the types of channel
/// </summary>
public enum ChannelTypes : short
{
    /// <summary>
    /// A personal channel is a channel between two persons, more like an individual chat
    /// </summary>
    Personal,

    /// <summary>
    /// A private channel is only on invitation, but it's a group chat. In example mentors talking with each other
    /// </summary>
    Private,

    /// <summary>
    /// A public channel can be read by everybody. In example system message from the admin to all the users
    /// </summary>
    Public,
}