
namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateSingleUserPropertyHandler(IServiceProvider serviceProvider)
    : UpdateSingleEntityPropertyHandler<UpdateSingleUserPropertyRequest, User>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateSingleUserPropertyRequest request, CancellationToken token)
    {
        var result = await base.Handle(request, token);
        if (result) Cache.Remove("database-user-" + request.Entity.Id);
        return result;
    }
}