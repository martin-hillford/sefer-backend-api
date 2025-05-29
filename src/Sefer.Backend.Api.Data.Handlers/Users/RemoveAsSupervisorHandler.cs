namespace Sefer.Backend.Api.Data.Handlers.Users;

public class RemoveAsSupervisorHandler(IServiceProvider serviceProvider)
    : Handler<RemoveAsSupervisorRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(RemoveAsSupervisorRequest request, CancellationToken token)
    {
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        if (user?.IsSupervisor != true) return false;

        // Todo: implement supervisor specific removal
        Cache.Remove("database-user-" + request.UserId);
        return await Send(new RemoveAsMentorRequest(request.UserId), token);
    }
}