namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class AddLogHandler(IServiceProvider serviceProvider) : Handler<AddLogRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(AddLogRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        context.Logs.Add(request.Log);
        context.SaveChanges();
        return Task.FromResult(true);
    }
}