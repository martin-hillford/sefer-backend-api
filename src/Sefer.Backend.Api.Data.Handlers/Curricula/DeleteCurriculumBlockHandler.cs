namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class DeleteCurriculumBlockHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteCurriculumBlockRequest, CurriculumBlock>(serviceProvider)
{
    public override Task<bool> Handle(DeleteCurriculumBlockRequest request, CancellationToken token)
    {
        var deleted = HandleSync(request);
        return Task.FromResult(deleted);
    }

    private bool HandleSync(DeleteCurriculumBlockRequest request)
    {
        var context = GetDataContext();
        using var transaction = context.BeginTransaction();

        try
        {
            var block = context.CurriculumBlocks.SingleOrDefault(b => b.Id == request.EntityId);
            if (block == null) return false;

            var blockCourses = context.CurriculumBlockCourses.Where(b => b.BlockId == request.EntityId).ToList();
            context.RemoveRange(blockCourses);
            context.Remove(block);

            context.SaveChanges();
            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            return false;
        }
    }
}