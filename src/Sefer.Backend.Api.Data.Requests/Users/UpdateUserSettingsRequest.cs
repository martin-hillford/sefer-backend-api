using System.Linq;

namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateUserSettingsRequest : IRequest<bool>
{
    public UpdateUserSettingsRequest(int userId, Dictionary<string, string> settings, string[] allowed)
    {
        UserId = userId;
        Settings = settings.AsReadOnly();
        Allowed = allowed.AsReadOnly();
    }
    
    public UpdateUserSettingsRequest(int userId, List<UserSetting> settings)
    {
        UserId = userId;
        Settings = settings.ToDictionary(s => s.KeyName, s => s.Value).AsReadOnly();
        Allowed = settings.Select(s => s.KeyName).ToList().AsReadOnly();
    }

    public readonly ReadOnlyDictionary<string, string> Settings;

    public readonly ReadOnlyCollection<string> Allowed;
    
    public readonly int UserId;
}   