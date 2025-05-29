namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class DeleteContentBlockHandler(IServiceProvider serviceProvider)
    : Handler<DeleteContentBlockRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(DeleteContentBlockRequest request, CancellationToken token)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(DeleteContentBlockRequest request)
    {
        var context = GetDataContext();
        var keyValues = new object[] { request.ContentBlockId };
        var block = context.Find(request.ContentBlockType, keyValues) as IContentBlock;
        if (block == null) return false;
        context.Remove(block);
        context.SaveChanges();
        return true;
    }
}