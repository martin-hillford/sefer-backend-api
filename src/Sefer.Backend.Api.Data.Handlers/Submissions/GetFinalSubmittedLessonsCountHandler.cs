namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetFinalSubmittedLessonsCountHandler(IServiceProvider serviceProvider)
    : Handler<GetFinalSubmittedLessonsCountRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetFinalSubmittedLessonsCountRequest request, CancellationToken token)
    {
        var enrollment = await Send(new GetEnrollmentByIdRequest(request.EnrollmentId), token);
        if (enrollment == null) return 0;
        var context = GetDataContext();
        return await context.LessonSubmissions.CountAsync(l => l.EnrollmentId == enrollment.Id && l.IsFinal, token);
    }
}