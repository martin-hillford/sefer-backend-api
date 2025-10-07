namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionsByTimeHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionsByTimeRequest, List<LessonSubmission>>(serviceProvider)
{
    public override Task<List<LessonSubmission>> Handle(GetSubmissionsByTimeRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return context.LessonSubmissions
            .AsNoTracking()
            .Where(s => s.Enrollment.StudentId == request.StudentId && s.SubmissionDate >= request.Start && s.IsFinal)
            .Include(s => s.Enrollment)
            .Include(s => s.Answers)
            .ToListAsync(token);
    }
}