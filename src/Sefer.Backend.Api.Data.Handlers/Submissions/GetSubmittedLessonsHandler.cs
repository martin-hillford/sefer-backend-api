namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmittedLessonsHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmittedLessonsRequest, List<LessonSubmission>>(serviceProvider)
{
    public override async Task<List<LessonSubmission>> Handle(GetSubmittedLessonsRequest request, CancellationToken token)
    {
        if (request.Top < 1) return new List<LessonSubmission>();
        var top = (int)request.Top;

        var context = GetDataContext();
        return await context.LessonSubmissions
            .AsNoTracking()
            .Where(e => e.Enrollment.StudentId == request.UserId && e.IsFinal == true)
            .Include(l => l.Lesson)
            .ThenInclude(s => s.CourseRevision)
            .ThenInclude(c => c.Course)
            .OrderByDescending(l => l.SubmissionDate)
            .Take(top)
            .ToListAsync(token);
    }
}