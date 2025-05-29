namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetClosedCurriculumRevisionsHandler(IServiceProvider serviceProvider)
    : Handler<GetClosedCurriculumRevisionsRequest, List<CurriculumRevision>>(serviceProvider)
{
    public override async Task<List<CurriculumRevision>> Handle(GetClosedCurriculumRevisionsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CurriculumRevisions
            .AsNoTracking()
            .Where(r => r.Stage == Stages.Closed && r.CurriculumId == request.CurriculumId)
            .ToListAsync(token);
    }
}