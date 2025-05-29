namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculaExtendedHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculaExtendedRequest, List<Curriculum>>(serviceProvider)
{
    public override async Task<List<Curriculum>> Handle(GetCurriculaExtendedRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Curricula
            .Include(c => c.Revisions)
            .ThenInclude(r => r.Blocks)
            .ThenInclude(b => b.BlockCourses)
            .AsNoTracking()
            .ToListAsync(cancellationToken: token);
    }
}