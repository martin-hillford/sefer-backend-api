namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class GetTemplatesHandler(IServiceProvider serviceProvider) : Handler<GetTemplatesRequest, List<Template>>(serviceProvider)
{
    public override async Task<List<Template>> Handle(GetTemplatesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Templates.ToListAsync(token);
    }
}