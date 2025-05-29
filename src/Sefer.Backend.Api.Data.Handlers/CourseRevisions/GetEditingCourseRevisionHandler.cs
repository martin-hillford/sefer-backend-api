namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetEditingCourseRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetEditingCourseRevisionRequest, CourseRevision>(serviceProvider)
{
    public override async Task<CourseRevision> Handle(GetEditingCourseRevisionRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions
            .AsNoTracking()
            .Where(r => (r.Stage == Stages.Edit || r.Stage == Stages.Test) && r.CourseId == request.CourseId)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(token);
    }
}