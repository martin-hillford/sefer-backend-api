namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionWithEnrollmentByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionWithEnrollmentByIdRequest, LessonSubmission>(serviceProvider)
{
    public override async Task<LessonSubmission> Handle(GetSubmissionWithEnrollmentByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.LessonSubmissions
            .AsNoTracking()
            .Include(l => l.Enrollment)
            .SingleOrDefaultAsync(l => l.Id == request.SubmissionId, token);
    }
}