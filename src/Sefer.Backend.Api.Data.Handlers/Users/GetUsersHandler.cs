namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUsersHandler(IServiceProvider serviceProvider) : Handler<GetUsersRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetUsersRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync(token);
    }
}