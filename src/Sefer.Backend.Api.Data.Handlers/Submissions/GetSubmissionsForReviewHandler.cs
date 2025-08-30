namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionsForReviewHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionsForReviewRequest, List<LessonSubmission>>(serviceProvider)
{
    public override async Task<List<LessonSubmission>> Handle(GetSubmissionsForReviewRequest request, CancellationToken token)
    {
        var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (mentor == null) return [];

        var context = GetDataContext();
        return await context.LessonSubmissions
            .AsNoTracking()
            .Where(l => l.Enrollment.MentorId == mentor.Id &&
                        l.SubmissionDate.HasValue &&
                        !l.ResultsStudentVisible &&
                        l.IsFinal)
            .Include(s => s.Enrollment).ThenInclude(e => e.Student)
            .Include(s => s.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(s => s.Lesson)
            .OrderBy(s => s.SubmissionDate)
            .ToListAsync(token);
    }
}