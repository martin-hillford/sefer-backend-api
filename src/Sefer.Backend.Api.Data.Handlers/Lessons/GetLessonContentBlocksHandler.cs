namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public abstract class GetLessonContentBlocksHandler<TRequest, TContentBlock>(IServiceProvider serviceProvider)
    : Handler<TRequest, List<TContentBlock>>(serviceProvider)
    where TRequest : GetLessonContentBlocksRequest<TContentBlock>
    where TContentBlock : class, IContentBlock<Lesson, TContentBlock>
{
    public override async Task<List<TContentBlock>> Handle(TRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var entitySet = context.Set<TContentBlock>();

        return await entitySet
            .AsNoTracking()
            .Where(b => b.LessonId == request.LessonId)
            .OrderBy(b => b.SequenceId)
            .ToListAsync(token);
    }
}