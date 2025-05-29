namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUserSettingsHandler(IServiceProvider serviceProvider)
    : Handler<GetUserSettingsRequest, List<UserSetting>>(serviceProvider)
{
    public override async Task<List<UserSetting>> Handle(GetUserSettingsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.UserSettings.AsNoTracking().Where(x => x.UserId == request.UserId).ToListAsync(token);
    }
}