namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class IsSubmissionReviewableHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle_NotFinal()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);

        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submission);
        await Handle(submission.Id, mentor.Id, false);
    }

    [TestMethod]
    public async Task Handle_NotMentor()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submission);
        await Handle(submission.Id, 19, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);

        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = true, LessonId = lesson.Id, };
        await InsertAsync(submission);
        await Handle(submission.Id, mentor.Id, true);
    }

    private async Task Handle(int submissionId, int mentorId, bool expected)
    {
        var request = new IsSubmissionReviewableRequest(mentorId, submissionId);
        var handler = new IsSubmissionReviewableHandler(ServiceProvider);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, result);
    }
}