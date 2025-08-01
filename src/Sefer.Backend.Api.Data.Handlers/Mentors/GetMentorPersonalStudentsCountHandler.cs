namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorPersonalStudentsCountHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorPersonalStudentsCountRequest, IReadOnlyDictionary<User, int>>(serviceProvider)
{
    public override async Task<IReadOnlyDictionary<User, int>> Handle(GetMentorPersonalStudentsCountRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var count = await context.StudentSettings 
            .Where(c => c.PersonalMentorId != null)
            .GroupBy(c => c.PersonalMentorId).Select(c => new { MentorId = c.Key.Value, Count = c.Count() })
            .ToDictionaryAsync(c => c.MentorId, c => c.Count, token);

        var mentors = await context.Users.Where(u => u.Role == UserRoles.Mentor).ToListAsync(token);
        
        return mentors.ToDictionary(mentor => mentor, mentor => count.GetValueOrDefault(mentor.Id, 0));
    }
}