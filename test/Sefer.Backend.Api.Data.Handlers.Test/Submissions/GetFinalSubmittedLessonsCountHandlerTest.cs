namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetFinalSubmittedLessonsCountHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle_NoEnrollment()
    {
        await Handle(null, -1, 0);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var provider = GetServiceProvider();
        provider.AddRequestResult<GetEnrollmentByIdRequest, Enrollment>(enrollment);

        await InsertAsync(new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, });
        await InsertAsync(new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = true, LessonId = lesson.Id, });

        await Handle(provider, enrollment.Id, 1);
    }

    private async Task Handle(MockedServiceProvider? serviceProvider, int enrollmentId, int expected)
    {
        var request = new GetFinalSubmittedLessonsCountRequest(enrollmentId);
        var handler = new GetFinalSubmittedLessonsCountHandler(serviceProvider?.Object ?? ServiceProvider);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, result);
    }
}