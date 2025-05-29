
namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class DeleteContentPageHandler(IServiceProvider serviceProvider) : Handler<DeleteContentPageRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteContentPageRequest request, CancellationToken token)
    {
        try
        {
            await using var context = GetDataContext();

            var contentPage = await context.ContentPages.SingleOrDefaultAsync(p => p.Id == request.Id, token);
            if (contentPage == null) return false;

            var specificPages = await context.SiteSpecificContentPages.Where(p => p.ContentPageId == request.Id).ToListAsync(token);
            if (specificPages.Count != 0)
            {
                context.RemoveRange(specificPages);
                await context.SaveChangesAsync(token);
            }

            context.Remove(contentPage);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}