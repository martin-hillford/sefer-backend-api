// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.Models.Users.Chat;
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Chat;

/// <summary>
/// A view on the channel
/// </summary>
public class ChannelView : Shared.Users.Chat.ChannelView
{
    /// <summary>
    /// The number of unread message in the channel
    /// </summary>
    public readonly int UnreadCount;

    /// <summary>
    /// Holds of the user has post rights in this channel
    /// </summary>
    public readonly bool HasPostRights;

    /// <summary>
    /// The name of channel (when it is not a personal channel)
    /// </summary>
    public new readonly string Name;

    /// <summary>
    /// A list of all the receivers in a channel
    /// </summary>
    public readonly List<ChannelReceiver> Receivers;

    /// <summary>
    /// Creates new view based on a model
    /// </summary>
    public ChannelView(Channel model, User user, int unreadCount, IAvatarService avatarService) : base(model)
    {
        Name = model.Name;
        if (model.Type == ChannelTypes.Personal)
        {
            Name = model.Receivers.FirstOrDefault(r => r.UserId != user.Id)?.User?.Name ?? "Unknown";
        }
        UnreadCount = unreadCount;
        HasPostRights = Type == ChannelTypes.Personal || Type == ChannelTypes.Private || user.Role == UserRoles.Admin;
        Receivers = model.Receivers.Select(r => new ChannelReceiver(r.User, avatarService)).ToList();
    }
}