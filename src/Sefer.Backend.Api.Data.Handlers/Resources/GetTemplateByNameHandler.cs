namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class GetTemplateByNameHandler(IServiceProvider serviceProvider) : Handler<GetTemplateByNameRequest, Template>(serviceProvider)
{
    public override async Task<Template> Handle(GetTemplateByNameRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Templates
            .SingleOrDefaultAsync(t => t.Name == request.Name && t.Language == request.Language && t.Type == request.GetTemplateType(), token);
    }
}