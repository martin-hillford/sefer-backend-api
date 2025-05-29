namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUsersByRoleHandler(IServiceProvider serviceProvider)
    : Handler<GetUsersByRoleRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetUsersByRoleRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Role == request.Role)
            .ToListAsync(token);

    }
}