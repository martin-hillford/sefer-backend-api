namespace Sefer.Backend.Api.Data.Requests.Users;

public class DisableTwoFactorAuthRequest(int userId) : IRequest<bool>
{
    public readonly int UserId = userId;
}