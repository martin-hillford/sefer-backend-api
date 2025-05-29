namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUserByEmailHandler(IServiceProvider serviceProvider)
    : Handler<GetUserByEmailRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetUserByEmailRequest request, CancellationToken token)
    {
        var email = request.Email?.ToLower().Trim();
        if (string.IsNullOrEmpty(email)) return null;

        var context = GetDataContext();
        return await context.Users
            .SingleOrDefaultAsync(u => u.Email.ToLower().Trim() == email, token);
    }
}