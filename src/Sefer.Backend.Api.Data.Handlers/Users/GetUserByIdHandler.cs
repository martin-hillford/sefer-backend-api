namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUserByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetUserByIdRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetUserByIdRequest request, CancellationToken token)
    {
        var cached = Cache.Get<User>("database-user-" + request.Id);
        if (cached != null) return cached;

        var timeout = DateTimeOffset.UtcNow.AddMinutes(5);
        var user = await base.Handle(request, token);
        Cache.Set("database-user-" + request.Id, user, timeout);
        return user;
    }
}


