namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class GetLoggedInfoByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetLoggedInfoByIdRequest, Log>(serviceProvider)
{
    public override Task<Log> Handle(GetLoggedInfoByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return context.Logs.SingleOrDefaultAsync(l => l.Id == request.Id, token);
    }
}