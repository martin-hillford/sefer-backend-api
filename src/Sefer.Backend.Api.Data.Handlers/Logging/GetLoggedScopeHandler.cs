namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class GetLoggedScopeHandler(IServiceProvider serviceProvider)
    : Handler<GetLoggedScopeRequest, List<Log>>(serviceProvider)
{
    public override Task<List<Log>> Handle(GetLoggedScopeRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return context.Logs
            .Where(l => l.Scope == request.ScopeId.ToString())
            .OrderBy(l => l.Timestamp)
            .ToListAsync(token);
    }
}