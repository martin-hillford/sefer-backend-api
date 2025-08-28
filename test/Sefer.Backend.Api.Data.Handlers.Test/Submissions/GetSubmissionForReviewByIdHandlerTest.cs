namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionForReviewByIdHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        var context = GetDataContext();
        await InitializeUsers(context);
        await InitializeSubmission(context);
    }

    [TestMethod]
    public async Task Handle_SubmissionIdNotMatching()
    {
        var submission = await HandleRequest(null, -1);
        Assert.IsNull(submission);
    }

    [TestMethod]
    public async Task Handle_MentorIdNotMatching()
    {
        var context = GetDataContext();
        var student = await context.GetStudent();
        var submission = await HandleRequest(student.Id);
        Assert.IsNull(submission);
    }

    [TestMethod]
    public async Task Handle_SubmissionNotFinal()
    {
        await SetValues(false, false, false);
        var submission = await HandleRequest();
        Assert.IsNull(submission);
    }

    [TestMethod]
    public async Task Handle_SubmissionAlreadyVisible()
    {
        await SetValues(true, true, false);
        var submission = await HandleRequest();
        Assert.IsNull(submission);
    }

    [TestMethod]
    public async Task Handle_SubmissionImported()
    {
        await SetValues(true, true, false);
        var submission = await HandleRequest();
        Assert.IsNull(submission);
    }

    [TestMethod]
    public async Task Handle()
    {
        await SetValues(true, false, false);
        var submission = await HandleRequest();
        Assert.IsNotNull(submission);
    }

    private async Task SetValues(bool final, bool visible, bool imported)
    {
        var context = GetDataContext();
        var submission = await context.LessonSubmissions.FirstAsync();

        submission.IsFinal = final;
        submission.ResultsStudentVisible = visible;
        submission.Imported = imported;

        context.Update(submission);
        await context.SaveChangesAsync();
    }

    private async Task<LessonSubmission> HandleRequest(int? mentorId = null, int? submissionId = null)
    {
        var context = GetDataContext();
        var lesson = await context.Lessons.FirstAsync();

        var provider = GetServiceProvider()
            .AddRequestResult<GetLessonIncludeReferencesRequest, Lesson>(lesson);
        var mentor = await context.GetMentor();
        var submission = await context.LessonSubmissions.FirstAsync();

        var request = new GetSubmissionForReviewByIdRequest(mentorId ?? mentor.Id, submissionId ?? submission.Id);
        var handler = new GetSubmissionForReviewByIdHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}