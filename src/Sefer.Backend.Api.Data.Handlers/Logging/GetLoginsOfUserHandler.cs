namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class GetLoginsOfUserHandler(IServiceProvider serviceProvider)
    : Handler<GetLoginsOfUserRequest, List<LoginLogEntry>>(serviceProvider)
{
    public override async Task<List<LoginLogEntry>> Handle(GetLoginsOfUserRequest request, CancellationToken token)
    {
        if (request.Top < 1) return [];
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        var context = GetDataContext();
        return await context.LoginLogEntries
            .AsNoTracking()
            .Where(login => login.Username == user.Email.ToLower())
            .OrderByDescending(login => login.LogTime)
            .Take(request.Top)
            .ToListAsync(token);
    }
}