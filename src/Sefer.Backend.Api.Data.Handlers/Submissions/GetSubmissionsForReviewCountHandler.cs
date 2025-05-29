namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionsForReviewCountHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionsForReviewCountRequest, long?>(serviceProvider)
{
    public override async Task<long?> Handle(GetSubmissionsForReviewCountRequest request, CancellationToken token)
    {
        var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (mentor == null) return null;
        var context = GetDataContext();
        return await context.LessonSubmissions
            .CountAsync(l =>
                l.Enrollment.MentorId == mentor.Id &&
                l.IsFinal &&
                !l.ResultsStudentVisible, token);
    }
}