namespace Sefer.Backend.Api.Data.Handlers.Users;


public class UpdateUserLastActivityHandler(IServiceProvider serviceProvider)
    : Handler<UpdateUserLastActivityRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateUserLastActivityRequest request, CancellationToken token)
    {
        try
        {
            await using var context = GetDataContext();
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            var query = $"UPDATE user_last_activity SET activity_date = '{time}' WHERE user_id = {request.UserId}";
            await context.Database.ExecuteSqlRawAsync(query, token);
            return true;
        }
        catch (Exception) { return false; }
    }
}