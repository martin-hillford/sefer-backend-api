namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateUserSettingsHandler(IServiceProvider serviceProvider)
    : Handler<UpdateUserSettingsRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateUserSettingsRequest request, CancellationToken token)
    {
        try
        {
            // Check if the provider userId exists in the database
            var context = GetDataContext();
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == request.UserId, token);
            if (user == null) return false;

            // Load the current present user settings
            var settings = await context.UserSettings
                .Where(x => x.UserId == request.UserId).ToDictionaryAsync(x => x.KeyName, token);

            // For each given setting check if it should be inserted or updated
            foreach (var (key, value) in request.Settings)
            {
                if (!request.Allowed.Contains(key)) continue;
                if (settings.TryGetValue(key, out var setting)) setting.Value = value;
                else
                    context.UserSettings.Add(new UserSetting { KeyName = key, Value = value, UserId = request.UserId });
            }

            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}