namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorsWithSettingsRequest(UserRoles role) : IRequest<List<User>>
{
    public readonly UserRoles Role = role;
}

