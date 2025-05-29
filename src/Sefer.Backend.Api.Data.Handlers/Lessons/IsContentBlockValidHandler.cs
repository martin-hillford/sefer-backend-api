namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class IsContentBlockValidHandler(IServiceProvider serviceProvider)
    : Handler<IsContentBlockValidRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsContentBlockValidRequest request, CancellationToken token)
    {
        // for a null object return false
        if (request.ContentBlock == null) return false;

        // When it's an update situation (id != 0) check if the contentBlock exists
        if (request.ContentBlock.Id <= 0) return await IsValidAsync(request.ContentBlock);
        
        var context = GetDataContext();
        var keyValues = new object[] { request.ContentBlock.Id };
        var dbBlock = await context.FindAsync(request.ContentBlockType, keyValues, token) as IContentBlock;
        if (dbBlock?.LessonId != request.LessonId) return false;
        await context.DisposeAsync();

        // return if the content block is valid
        return await IsValidAsync(request.ContentBlock);
    }
}