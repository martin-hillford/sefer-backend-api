namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetPublicAdminChannelHandler(IServiceProvider serviceProvider)
    : Handler<GetPublicAdminChannelRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(GetPublicAdminChannelRequest request, CancellationToken token)
    {
        // CHeck if the user is not an admin user
        var user = await Send(new GetUserByIdRequest(request.User), token);
        if (user == null || user.Role == UserRoles.Admin) return null;

        // Check if the user is already speaking with an admin
        var channel = await GetAdminChannel(user.Id, token);
        if (channel != null) return channel;

        // Select random a public admin for the admin users
        var admin = await Send(new GetPublicAdminRequest(), token);
        if (admin == null) return null;

        return await Send(new CreateChannelRequest(user.Id, admin.Id), token);
    }

    private async Task<Channel> GetAdminChannel(int userId, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.ChatChannels
            .AsNoTracking()
            .Where(c => c.Type == ChannelTypes.Personal &&
                        c.Receivers.All(r =>
                            r.UserId == userId ||
                            context.AdminSettings.Any(a => a.AdminId == r.UserId && a.IsPublicAdmin)) &&
                        c.Receivers.Count == 2)
            .Include(c => c.Receivers)
            .FirstOrDefaultAsync(token);
    }
}