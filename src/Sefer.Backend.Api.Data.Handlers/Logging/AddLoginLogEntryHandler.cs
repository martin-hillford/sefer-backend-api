namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class AddLoginLogEntryHandler(IServiceProvider serviceProvider)
    : Handler<AddLoginLogEntryRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(AddLoginLogEntryRequest request, CancellationToken token)
    {
        var valid = IsValidEntity(request.LoginLogEntry);
        if (!valid) return Task.FromResult(false);

        var context = GetDataContext();
        context.LoginLogEntries.Add(request.LoginLogEntry);
        context.SaveChanges();
        return Task.FromResult(true);
    }
}