namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class IsSubmissionReviewableHandler(IServiceProvider serviceProvider)
    : Handler<IsSubmissionReviewableRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsSubmissionReviewableRequest request, CancellationToken token)
    {
        // Please note, including the mentor id is an additional check
        var context = GetDataContext();
        return await context.LessonSubmissions
            .AnyAsync(s => s.Id == request.SubmissionId && s.Enrollment.MentorId == request.MentorId && s.IsFinal, token);
    }
}