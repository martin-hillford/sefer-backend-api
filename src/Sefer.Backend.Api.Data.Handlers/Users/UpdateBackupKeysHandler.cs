namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateBackupKeysHandler(IServiceProvider serviceProvider)
    : Handler<UpdateBackupKeysRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(UpdateBackupKeysRequest request, CancellationToken token)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(UpdateBackupKeysRequest request)
    {
        {
            try
            {
                Cache.Remove("database-user-" + request.UserId);

                var context = GetDataContext();
                var existingKeys = context.UserBackupKeys.Where(k => k.UserId == request.UserId);
                context.RemoveRange(existingKeys);
                context.SaveChanges();

                var keys = request.Keys.Select(k => new UserBackupKey { BackupKey = Hashing.Sha512(k), UserId = request.UserId });
                context.UserBackupKeys.AddRange(keys);
                context.SaveChanges();
            }
            catch (Exception) { return false; }
            return true;
        }
    }
}