namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorsRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetMentorsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Role != UserRoles.User && u.Role != UserRoles.Student && u.Role != UserRoles.CourseMaker)
            .Include(m => m.MentorSettings)
            .OrderBy(u => u.Name)
            .ToListAsync(token);
    }
}