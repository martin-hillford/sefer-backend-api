namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUsersCountHandler(IServiceProvider serviceProvider) : Handler<GetUsersCountRequest, long>(serviceProvider)
{
    public override async Task<long> Handle(GetUsersCountRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Users.AsNoTracking().LongCountAsync(token);
    }
}