// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Chat;

/// <summary>
/// Simple view for the users that are in a channel
/// </summary>
public class ChannelReceiver
{
    /// <summary>
    /// The id of the user in the channel
    /// </summary>
    public readonly int UserId;

    /// <summary>
    /// The name of the user in the channel
    /// </summary>
    public readonly string UserName;

    /// <summary>
    /// The avatar url for the user
    /// </summary>
    public readonly string UserAvatarUrl;

    /// <summary>
    /// Holds if the user is active will be only set for mentors
    /// </summary>
    public bool? UserActive;

    /// <summary>
    /// Create a ChannelReceiver
    /// </summary>
    public ChannelReceiver(User user, IAvatarService avatarService)
    {
        UserId = user.Id;
        UserName = user.Name;
        UserAvatarUrl = avatarService.GetAvatarUrl(UserId, UserName);
    }
}