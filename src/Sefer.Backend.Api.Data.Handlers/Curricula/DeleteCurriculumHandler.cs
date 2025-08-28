namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class DeleteCurriculumHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteCurriculumRequest, Curriculum>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteCurriculumRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        await using var transaction = context.BeginTransaction();
        try
        {
            var curriculum = await context.Curricula
                .Where(c => c.Id == request.EntityId).Include(c => c.Revisions)
                .SingleOrDefaultAsync(token);

            if (curriculum?.IsEditable != true) return false;
            context.Curricula.Remove(curriculum);
            await context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            return false;
        }
    }
}