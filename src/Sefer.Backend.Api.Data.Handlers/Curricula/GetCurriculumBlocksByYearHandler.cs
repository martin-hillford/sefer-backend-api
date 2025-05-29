namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumBlocksByYearHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumBlocksByYearRequest, List<CurriculumBlock>>(serviceProvider)
{
    public override async Task<List<CurriculumBlock>> Handle(GetCurriculumBlocksByYearRequest request, CancellationToken token)
    {
        var revision = await Send(new GetCurriculumRevisionByIdRequest(request.CurriculumRevisionId), token);
        if (revision == null) return new List<CurriculumBlock>();

        var context = GetDataContext();
        return await context.CurriculumRevisions
            .AsNoTracking()
            .Where(c => c.Id == revision.Id)
            .SelectMany(c => c.Blocks)
            .Where(b => b.Year == request.Year)
            .OrderBy(b => b.SequenceId)
            .ToListAsync(token);
    }
}