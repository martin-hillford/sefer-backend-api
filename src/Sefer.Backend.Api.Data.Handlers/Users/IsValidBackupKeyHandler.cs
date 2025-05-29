namespace Sefer.Backend.Api.Data.Handlers.Users;

public class IsValidBackupKeyHandler(IServiceProvider serviceProvider)
    : Handler<IsValidBackupKeyRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsValidBackupKeyRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var hashedKey = Hashing.Sha512(request.Key);
        return await context.UserBackupKeys.AnyAsync(k => k.UserId == request.UserId && k.BackupKey == hashedKey, token);
    }
}