namespace Sefer.Backend.Api.Data.Requests.Logging;

public class GetLoginsOfUserRequest(int userId, int top) : IRequest<List<LoginLogEntry>>
{
    public readonly int UserId = userId;

    public readonly int Top = top;
}