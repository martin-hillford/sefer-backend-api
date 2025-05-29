namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetFinalSubmissionByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetFinalSubmissionByIdRequest, LessonSubmission>(serviceProvider)
{
    public override async Task<LessonSubmission> Handle(GetFinalSubmissionByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var submission = await context.LessonSubmissions
            .AsNoTracking()
            .Where(s => s.Id == request.SubmissionId && s.IsFinal)
            .Include(s => s.Enrollment).ThenInclude(e => e.Student)
            .Include(s => s.Enrollment).ThenInclude(e => e.Mentor)
            .Include(s => s.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(s => s.Answers)
            .FirstOrDefaultAsync(token);

        if (submission == null) return null;
        submission.Lesson = await Send(new GetLessonIncludeReferencesRequest(submission.LessonId), token);
        return submission;
    }
}