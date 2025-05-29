namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseByIdExtendedHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseByIdExtendedRequest, Course>(serviceProvider)
{
    public override async Task<Course> Handle(GetCourseByIdExtendedRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var course = await context.Courses
            .AsNoTracking()
            .Where(c => c.Id == request.CourseId)
            .Include(c => c.CourseRevisions).ThenInclude(r => r.Lessons)
            .SingleOrDefaultAsync(token);

        if (course == null) return null;

        foreach (var revision in course.CourseRevisions)
        {
            if (revision.Lessons != null) revision.Lessons = revision.Lessons.OrderBy(l => l.SequenceId).ToList();
        }

        return course;
    }
}