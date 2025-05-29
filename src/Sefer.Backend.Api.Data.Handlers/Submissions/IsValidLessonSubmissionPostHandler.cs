namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class IsValidLessonSubmissionPostHandler(IServiceProvider serviceProvider)
    : Handler<IsValidLessonSubmissionPostRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsValidLessonSubmissionPostRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var submission = await context.LessonSubmissions
            .Where(l => l.Id == request.LessonSubmissionId)
            .Include(l => l.Enrollment).ThenInclude(e => e.Mentor)
            .Include(l => l.Enrollment).ThenInclude(e => e.Student)
            .SingleOrDefaultAsync(token);

        if (submission?.IsFinal != true) return false;
        if (submission.Enrollment?.IsSelfStudy != false) return false;
        if (submission.Enrollment?.Mentor?.IsMentor != true) return false;
        if (submission.Enrollment?.Student == null) return false;

        // Ensure a channel exists between the mentor and the student
        var getChannel = new GetPersonalChannelRequest(submission.Enrollment.MentorId.Value, submission.Enrollment.StudentId);
        var chanel = Send(getChannel, token);
        return chanel != null;
    }
}