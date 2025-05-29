namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateUserSettingsRequest(int userId, Dictionary<string, string> settings, string[] allowed) : IRequest<bool>
{
    public readonly ReadOnlyDictionary<string, string> Settings = settings.AsReadOnly();
    
    public readonly ReadOnlyCollection<string> Allowed = allowed.AsReadOnly();
    
    public readonly int UserId = userId;
}   