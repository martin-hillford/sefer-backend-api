namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetClosedCourseRevisionsHandler(IServiceProvider serviceProvider)
    : Handler<GetClosedCourseRevisionsRequest, List<CourseRevision>>(serviceProvider)
{
    public override async Task<List<CourseRevision>> Handle(GetClosedCourseRevisionsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions
            .AsNoTracking()
            .Where(r => r.Stage == Stages.Closed && r.CourseId == request.CourseId)
            .Include(r => r.Course)
            .ToListAsync(token);
    }
}