
namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateUserHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateUserRequest, User>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateUserRequest request, CancellationToken token)
    {
        var result = await base.Handle(request, token);
        if (result) Cache.Remove("database-user-" + request.Entity.Id);
        return result;
    }
}