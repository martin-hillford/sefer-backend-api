// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Shared.Users;

/// <summary>
/// An extended view of the profile
/// </summary>
public class ProfileInfoView : UserView
{
    public readonly string AvatarUrl;
    
    public Dictionary<string, string> Settings { get; set; }

    public ProfileInfoView(User user, List<UserSetting> settings, IAvatarService avatarService) : base(user)
    {
        AvatarUrl = avatarService.GetNonCachedAvatarUrl(Id, Name);
        Settings = settings.ToDictionary(s => s.KeyName, s => s.Value);
    }
}

