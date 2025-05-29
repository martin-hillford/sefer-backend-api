namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorRatingsRequest(UserRoles role) : IRequest<Dictionary<int, Tuple<int, int>>>
{
    public readonly UserRoles Role = role;
}