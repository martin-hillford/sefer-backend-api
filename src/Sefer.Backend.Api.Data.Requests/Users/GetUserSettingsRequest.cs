namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetUserSettingsRequest(int userId) : IRequest<List<UserSetting>>
{
    public readonly int UserId = userId;
}