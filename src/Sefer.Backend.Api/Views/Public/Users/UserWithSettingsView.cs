// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Public.Users;

public class UserWithSettingsView(User user, List<UserSetting> settings) : UserView(user)
{
    private readonly User _user = user;

    public Dictionary<string, string> Settings {
        get
        {
            var dictionary = settings.ToDictionary(s => s.KeyName, s => s.Value);
            dictionary.Add("notificationPreference", _user.NotificationPreference.GetLabel());
            dictionary.Add("preferredInterfaceLanguage", _user.PreferredInterfaceLanguage.ToLower());
            dictionary.Add("preferSpokenCourses", _user.PreferSpokenCourses ? "true" : "false");
            dictionary.Add("allowImpersonation", _user.AllowImpersonation ? "true" : "false");
            return dictionary;
        }
    }
}