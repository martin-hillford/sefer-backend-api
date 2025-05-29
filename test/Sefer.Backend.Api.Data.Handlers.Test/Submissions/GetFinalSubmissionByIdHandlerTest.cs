namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetFinalSubmissionByIdHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle_NotFinal()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submission);
        await Handle(submission.Id, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = true, LessonId = lesson.Id, };
        await InsertAsync(submission);
        await Handle(submission.Id, true);
    }

    private async Task Handle(int submissionId, bool expected)
    {
        var request = new GetFinalSubmissionByIdRequest(submissionId);
        var handler = new GetFinalSubmissionByIdHandler(ServiceProvider);
        var result = await handler.Handle(request, CancellationToken.None);

        if (!expected) Assert.IsNull(result);
        else Assert.IsNotNull(result);

    }
}