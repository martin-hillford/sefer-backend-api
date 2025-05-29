namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionWithEnrollmentByIdHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        var context = GetDataContext();
        await InitializeUsers(context);
        await InitializeSubmission(context);
    }

    [TestMethod]
    public async Task Handle_IdNotMatching()
    {
        var submission = await Handle(-1);
        submission.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var submission = await context.LessonSubmissions.FirstAsync();

        var result = await Handle(submission.Id);
        result.Should().NotBeNull();
        result.Enrollment.Should().NotBeNull();
    }

    private async Task<LessonSubmission> Handle(int submissionId)
    {
        var request = new GetSubmissionWithEnrollmentByIdRequest(submissionId);
        var provider = GetServiceProvider().Object;
        var handler = new GetSubmissionWithEnrollmentByIdHandler(provider);
        return await handler.Handle(request, CancellationToken.None);
    }
}