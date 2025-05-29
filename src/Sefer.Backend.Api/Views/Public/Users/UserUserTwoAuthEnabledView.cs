namespace Sefer.Backend.Api.Views.Public.Users;

public class UserUserTwoAuthEnabledView(User user)
{
    public bool Enabled { get; init; } = user.TwoFactorAuthEnabled;
}
