namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumByIdRequest, Curriculum>(serviceProvider)
{
    public override async Task<Curriculum> Handle(GetCurriculumByIdRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var query = context.Curricula.AsNoTracking();
        if (request.IncludeRevisions) query = query.Include(c => c.Revisions);
        return await query.SingleOrDefaultAsync(c => c.Id == request.Id, token);
    }
}