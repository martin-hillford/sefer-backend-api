namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCourseByPermalinkHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCourseByPermalinkRequest, Course>(serviceProvider)
{
    public override async Task<Course> Handle(GetPublishedCourseByPermalinkRequest request, CancellationToken token)
    {
        var permalink = request?.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return null;

        var context = GetDataContext();
        return await context.CourseRevisions
            .Where(r => r.Stage == Stages.Published && r.Course.Permalink.ToLower() == permalink)
            .Include(r => r.Course)
            .ThenInclude(c => c.CourseRevisions)
            .Select(r => r.Course)
            .SingleOrDefaultAsync(token);
    }
}