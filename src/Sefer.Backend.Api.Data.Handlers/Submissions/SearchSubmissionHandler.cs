namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class SearchSubmissionHandler(IServiceProvider serviceProvider)
    : Handler<SearchSubmissionRequest, LessonSubmission>(serviceProvider)
{
    public override async Task<LessonSubmission> Handle(SearchSubmissionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.LessonSubmissions
            .FirstOrDefaultAsync(x => x.EnrollmentId == request.EnrollmentId && x.LessonId == request.LessonId, token);
    }
}