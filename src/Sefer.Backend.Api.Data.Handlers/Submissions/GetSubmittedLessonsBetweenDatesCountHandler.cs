namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmittedLessonsBetweenDatesCountHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmittedLessonsBetweenDatesCountRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetSubmittedLessonsBetweenDatesCountRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.LessonSubmissions
            .AsNoTracking()
            .Where(e =>
                e.SubmissionDate.HasValue &&
                e.SubmissionDate.Value >= request.LowerBound &&
                e.SubmissionDate.Value <= request.UpperBound &&
                e.IsFinal &&
                e.Enrollment.StudentId == request.StudentId)
            .Select(s => s.SubmissionDate.Value)
            .CountAsync(token);
    }
}