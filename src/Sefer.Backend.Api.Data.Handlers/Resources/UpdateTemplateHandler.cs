namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class UpdateTemplateHandler(IServiceProvider serviceProvider) : Handler<UpdateTemplateRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateTemplateRequest request, CancellationToken token)
    {
        try
        {
            var context = GetDataContext();
            var template = await context.Templates.SingleOrDefaultAsync(t => t.Id == request.TemplateId, token);
            if (template == null) return false;
            
            template.Title = request.Title;
            template.Content = request.Content;
            
            context.Templates.Update(template);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}


