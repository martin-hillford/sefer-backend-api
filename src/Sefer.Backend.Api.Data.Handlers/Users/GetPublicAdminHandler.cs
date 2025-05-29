namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetPublicAdminHandler(IServiceProvider serviceProvider)
    : Handler<GetPublicAdminRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetPublicAdminRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var admins = context.AdminSettings
            .Where(a => a.IsPublicAdmin && a.Admin.Role == UserRoles.Admin);

        var count = await admins.CountAsync(token);
        if (count == 0) return null;

        var rand = new Random();
        var toSkip = rand.Next(0, count);

        return await admins.Select(a => a.Admin).Skip(toSkip).Take(1).FirstOrDefaultAsync(token);
    }
}


