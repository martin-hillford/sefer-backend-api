namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorsWithSettingsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorsWithSettingsRequest, List<User>>(serviceProvider), IRequest<List<User>>
{
    public override async Task<List<User>> Handle(GetMentorsWithSettingsRequest request, CancellationToken token)
    {
        if (request.Role is UserRoles.User or UserRoles.Student) return new List<User>();

        var context = GetDataContext();
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Role == request.Role)
            .Include(u => u.MentorSettings)
            .ToListAsync(token);
    }
}