namespace Sefer.Backend.Api.Data.Requests.Logging;

public class GetLoggedScopeRequest(Guid scopeId) : IRequest<List<Log>>
{
    public readonly Guid ScopeId = scopeId;
}