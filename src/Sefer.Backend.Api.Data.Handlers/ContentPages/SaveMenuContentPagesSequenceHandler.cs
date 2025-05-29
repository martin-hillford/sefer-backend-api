namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class SaveMenuContentPagesSequenceHandler(IServiceProvider serviceProvider)
    : SyncHandler<SaveMenuContentPagesSequenceRequest, bool>(serviceProvider)
{
    protected override bool Handle(SaveMenuContentPagesSequenceRequest request)
    {
        var contentPageIds = request.ContentPages?.Select(p => p.Id).ToList();

        // Check for a null reference
        if (contentPageIds == null) return false;

        // Get all the menu pages (published and unpublished)
        using var context = GetDataContext();
        var menuPages = context.ContentPages.Where(p => p.Type == ContentPageType.MenuPage).ToList();

        // Check if the submitted ids are matching the ones in the database
        if (contentPageIds.Count != menuPages.Count) return false;
        var intersection = contentPageIds.Intersect(menuPages.Select(p => p.Id)).ToList();
        if (intersection.Count != menuPages.Count) return false;

        // Create a lookup table for the menu pages
        var lookup = menuPages.ToDictionary(p => p.Id);

        // Now set the sequence id's of content pages
        for (var sequenceId = 0; sequenceId < contentPageIds.Count; sequenceId++)
        {
            var page = lookup[contentPageIds[sequenceId]];
            page.SequenceId = sequenceId;
        }

        context.ContentPages.UpdateRange(menuPages);
        context.SaveChanges();
        return true;
    }
}