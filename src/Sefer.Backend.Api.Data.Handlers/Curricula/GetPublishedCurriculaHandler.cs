namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetPublishedCurriculaHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCurriculaRequest, List<Curriculum>>(serviceProvider)
{
    public override async Task<List<Curriculum>> Handle(GetPublishedCurriculaRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var query = context.CurriculumRevisions
            .AsNoTracking()
            .Where(r => r.Stage == Stages.Published);

        if (request.IncludeCourses)
        {
            query = query
                .Include(r => r.Blocks)
                .ThenInclude(b => b.BlockCourses)
                .ThenInclude(b => b.Course)
                .ThenInclude(c => c.CourseRevisions);
        }

        var revisions = await query
            .Include(r => r.Curriculum)
            .OrderBy(r => r.Curriculum.Name)
            .ToListAsync(token);

        return revisions.Select(c => c.Curriculum).ToList();
    }
}