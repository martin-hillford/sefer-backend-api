namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetInactiveStudentsHandler(IServiceProvider serviceProvider)
    : Handler<GetInactiveStudentsRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetInactiveStudentsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var studentIds = context.UserLastActivities
            .Where(g => g.ActivityDate.Date == request.ActivityDate.AddDays(1).Date)
            .Select(u => u.UserId);

        return await context.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRoles.Student && studentIds.Contains(u.Id))
            .ToListAsync(token);
    }
}