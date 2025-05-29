namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateBackupKeysRequest(User user, List<string> keys) : IRequest<bool>
{
    public readonly int UserId = user.Id;

    public readonly List<string> Keys = keys;
}