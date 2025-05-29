namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetEditingCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetEditingCurriculumRevisionRequest, CurriculumRevision>(serviceProvider)
{
    public override async Task<CurriculumRevision> Handle(GetEditingCurriculumRevisionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var curriculum = await context.Curricula.SingleOrDefaultAsync(c => c.Id == request.CurriculumId, token);
        if (curriculum == null) return null;

        return await context.CurriculumRevisions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => (r.Stage == Stages.Edit || r.Stage == Stages.Test) && r.CurriculumId == curriculum.Id, token);
    }
}