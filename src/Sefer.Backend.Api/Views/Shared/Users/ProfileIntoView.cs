// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Shared.Users;

/// <summary>
/// An extended view of the profile
/// </summary>
public class ProfileInfoView : UserWithSettingsView
{
    public readonly string AvatarUrl;
    
    public ProfileInfoView(User user, List<UserSetting> settings, IAvatarService avatarService) : base(user, settings)
    {
        AvatarUrl = avatarService.GetNonCachedAvatarUrl(Id, Name);
    }
}

