namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

// Please note, LessonSubmission is a rather complex entity, therefore the generic UpdateHandlerTest is hard to reuse

[TestClass]
public class UpdateSubmissionHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        await base.Initialize();

        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var submission = new LessonSubmission
        {
            IsFinal = true,
            EnrollmentId = enrollment.Id,
            LessonId = lesson.Id,
            SubmissionDate = DateTime.Now.AddDays(-1)
        };
        await InsertAsync(submission);
    }

    [TestMethod]
    public async Task Handle_InvalidEntity()
    {
        await Handle(new LessonSubmission(), false);
    }

    [TestMethod]
    public async Task Handle_UpdateFailed()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();
        submission.EnrollmentId = -1;
        await Handle(submission, false);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();

        var provider = GetServiceProvider(new Exception());
        await Handle(submission, false, provider);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();
        await Handle(submission, true);
    }

    private async Task Handle(LessonSubmission entity, bool expectedResult, MockedServiceProvider? provider = null)
    {
        var request = new UpdateSubmissionRequest(entity);
        var serviceProvider = provider ?? GetServiceProvider();
        var handler = new UpdateSubmissionHandler(serviceProvider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expectedResult, result);
    }
}