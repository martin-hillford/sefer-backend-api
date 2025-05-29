namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculaHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculaRequest, List<Curriculum>>(serviceProvider)
{
    public override async Task<List<Curriculum>> Handle(GetCurriculaRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var query = context.Curricula.AsNoTracking();
        if (request.IncludeRevisions) query = query.Include(c => c.Revisions);
        return await query.OrderBy(c => c.Name).ToListAsync(token);
    }
}