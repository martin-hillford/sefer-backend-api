namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorPersonalStudentsCountRequest
    : IRequest<IReadOnlyDictionary<User, int>> { }