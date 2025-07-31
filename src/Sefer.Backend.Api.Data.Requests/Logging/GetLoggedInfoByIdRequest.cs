namespace Sefer.Backend.Api.Data.Requests.Logging;

public class GetLoggedInfoByIdRequest(Guid id) : IRequest<Log>
{
    public readonly Guid Id = id;
}