namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionByIdHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        var context = GetDataContext();
        await base.Initialize();

        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();
        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submission);
    }

    [TestMethod]
    public async Task Handle_NotFound()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();

        var result = await Handle(submission.Id + 1);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();

        var result = await Handle(submission.Id);
        Assert.IsNotNull(result);
    }
    private async Task<LessonSubmission> Handle(int submissionId)
    {
        var provider = GetServiceProvider();
        var request = new GetSubmissionByIdRequest(submissionId);
        var handler = new GetSubmissionByIdHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}