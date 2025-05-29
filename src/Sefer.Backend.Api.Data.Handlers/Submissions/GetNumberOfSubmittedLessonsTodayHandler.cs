namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetNumberOfSubmittedLessonsTodayHandler(IServiceProvider serviceProvider)
    : Handler<GetNumberOfSubmittedLessonsTodayRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetNumberOfSubmittedLessonsTodayRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.LessonSubmissions
            .CountAsync(s => s.Enrollment.StudentId == request.StudentId &&
                             s.IsFinal && s.SubmissionDate.HasValue &&
                             s.SubmissionDate.Value.Date == DateTime.UtcNow.Date, token);
    }
}