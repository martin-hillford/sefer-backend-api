namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class GetVersionInfoHandler(IServiceProvider serviceProvider)
    : Handler<GetVersionInfoRequest, VersionInfo>(serviceProvider)
{
    public override Task<VersionInfo> Handle(GetVersionInfoRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return Task.FromResult(new VersionInfo
        {
            Database = context.GetDatabaseVersion(),
            Provider = context.GetDatabaseProvider()
        });
    }
}