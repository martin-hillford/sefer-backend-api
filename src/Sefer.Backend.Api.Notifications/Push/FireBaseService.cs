using Sefer.Backend.Api.Data.Requests.CourseRevisions;
using Sefer.Backend.Api.Data.Requests.Courses;
using Sefer.Backend.Api.Data.Requests.Lessons;
using Sefer.Backend.Api.Data.Requests.Submissions;

namespace Sefer.Backend.Api.Notifications.Push;

public class FireBaseService(IMediator mediator, IFireBase push) : IFireBaseService
{
    private Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
        => mediator.Send(request, token);

    public async Task SendLessonSubmittedNotificationToMentor(int submissionId, User mentor, User student)
    {
        try
        {
            if (!await HasPushNotificationCapableDevice(mentor.Id)) return;

            var submission = await Send(new GetSubmissionWithEnrollmentByIdRequest(submissionId));
            var lesson = await Send(new GetLessonByIdRequest(submission.LessonId));
            var courseRevision = await Send(new GetCourseRevisionByIdRequest(submission.Enrollment.CourseRevisionId));
            var course = await Send(new GetCourseByIdRequest(courseRevision.CourseId));
            var language = mentor.GetPreferredInterfaceLanguage();
            var content = await Localization.GetContent(mediator, language, "LessonSubmitted");
            var vars = new Dictionary<string, string>
            {
                { "@student", student.Name }, { "@lesson", lesson.Number }, { "@course", course.Name }
            };
            var (title, body) = ReplaceVars(content, vars);

            await push.SendMessage(mentor.Id, title, body);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }

    public async Task SendLessonReviewedNotificationToStudent(int submissionId, User student)
    {
        try
        {
            if (!await HasPushNotificationCapableDevice(student.Id)) return;

            var submission = await Send(new GetSubmissionWithEnrollmentByIdRequest(submissionId));
            var lesson = await Send(new GetLessonByIdRequest(submission.LessonId));
            var courseRevision = await Send(new GetCourseRevisionByIdRequest(submission.Enrollment.CourseRevisionId));
            var course = await Send(new GetCourseByIdRequest(courseRevision.CourseId));
            var language = student.GetPreferredInterfaceLanguage();
            var content = await Localization.GetContent(mediator, language, "LessonReviewed");
            var vars = new Dictionary<string, string> { { "@lesson", lesson.Number }, { "@course", course.Name } };
            var (title, body) = ReplaceVars(content, vars);
            await push.SendMessage(student.Id, title, body);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }

    public async Task<bool> SendChatTextMessageNotification(int userId, string title, string body, bool throwExceptions)
    {
        var exceptions = await push.SendMessage(userId, title, body);
        if (exceptions.Count == 0) return true;
        return throwExceptions switch
        {
            true when exceptions.Count == 1 => throw exceptions[0],
            true => throw new AggregateException("Multiple Errors Occured", exceptions),
            _ => false
        };
    }

    public async Task SendStudentIsInactiveNotificationAsync(User student)
    {
        if (!await HasPushNotificationCapableDevice(student.Id)) return;
        var (title, body) = await Localization.GetContent(mediator, student.PreferredInterfaceLanguage, "MissingStudent");
        await push.SendMessage(student.Id, title, body);
    }

    public async Task SendStudentEnrolledNotificationToMentor(Course course, Enrollment enrollment)
    {
        if (!await HasPushNotificationCapableDevice(enrollment.Mentor.Id)) return;

        var language = enrollment.Mentor.GetPreferredInterfaceLanguage();
        var content = await Localization.GetContent(mediator, language, "NewEnrollment");

        var vars = new Dictionary<string, string> { { "@student", enrollment.Student.Name }, { "@course", course.Name } };
        var (title, body) = ReplaceVars(content, vars);
        await push.SendMessage(enrollment.Mentor.Id, title, body);
    }

    private async Task<bool> HasPushNotificationCapableDevice(int userId)
    {
        var tokens = await Send(new GetPushNotificationTokensByUserIdRequest(userId));
        return tokens.Count != 0;
    }

    public static (string title, string body) ReplaceVars((string Title, string Body) content, Dictionary<string, string> vars)
    {
        foreach (var (key, value) in vars)
        {
            content.Title = content.Title.Replace(key, value);
            content.Body = content.Body.Replace(key, value);
        }
        return content;
    }
}
