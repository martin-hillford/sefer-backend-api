
namespace Sefer.Backend.Api.Data.Handlers.Users;

public class DeleteUserHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteUserRequest, User>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteUserRequest request, CancellationToken token)
    {
        var result = await base.Handle(request, token);
        if (result) Cache.Remove("database-user-" + request.EntityId);
        return result;
    }
}