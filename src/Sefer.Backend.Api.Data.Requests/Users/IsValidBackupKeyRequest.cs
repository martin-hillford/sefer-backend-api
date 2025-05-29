namespace Sefer.Backend.Api.Data.Requests.Users;

public class IsValidBackupKeyRequest(int userId, string key) : IRequest<bool>
{
    public readonly int UserId = userId;

    public readonly string Key = key;
}