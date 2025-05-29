namespace Sefer.Backend.Api.Data.Handlers.Users;

public class DisableTwoFactorAuthHandler(IServiceProvider serviceProvider)
    : Handler<DisableTwoFactorAuthRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(DisableTwoFactorAuthRequest request, CancellationToken token)
    {
        Cache.Remove("database-user-" + request.UserId);
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(DisableTwoFactorAuthRequest request)
    {
        var context = GetDataContext();
        var user = context.Users.SingleOrDefault(u => u.Id == request.UserId);
        if (user == null) return false;

        user.TwoFactorAuthEnabled = false;
        context.UpdateSingleProperty(user, nameof(user.TwoFactorAuthEnabled));
        return true;
    }
}