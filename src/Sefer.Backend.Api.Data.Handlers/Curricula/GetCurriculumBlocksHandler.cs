namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumBlocksHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumBlocksRequest, List<CurriculumBlock>>(serviceProvider)
{
    public override async Task<List<CurriculumBlock>> Handle(GetCurriculumBlocksRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var revision = await context.CurriculumRevisions.SingleOrDefaultAsync(r => r.Id == request.CurriculumRevisionId, token);
        if (revision == null) return new List<CurriculumBlock>();

        var helper = new GetCurriculumBlocksHelper(context);
        return helper.Handle(revision);
    }
}