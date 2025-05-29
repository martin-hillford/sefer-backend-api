namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class IsValidLessonSubmissionPostHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        var context = GetDataContext();
        await InitializeUsers(context);
        await InitializeSubmission(context);
    }

    [TestMethod]
    public async Task Handle_NoSubmission()
    {
        var valid = await Handle(int.MaxValue);
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public async Task Handle_SubmissionNotFinal()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();
        submission.IsFinal = false;
        context.UpdateSingleProperty(submission, nameof(LessonSubmission.IsFinal));

        var valid = await Handle(submission.Id);
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public async Task Handle_NoMentor()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var submission = context.LessonSubmissions.Single();

        enrollment.MentorId = null;
        context.UpdateSingleProperty(enrollment, nameof(enrollment.MentorId));

        var valid = await Handle(submission.Id);
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public async Task Handle_MentorIsNoMentor()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var submission = context.LessonSubmissions.Single();

        enrollment.MentorId = enrollment.StudentId;
        context.UpdateSingleProperty(enrollment, nameof(enrollment.MentorId));

        var valid = await Handle(submission.Id);
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();

        var valid = await Handle(submission.Id);
        Assert.IsTrue(valid);
    }

    private async Task<bool> Handle(int submissionId)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetPersonalChannelRequest, Channel>(new Channel());

        var request = new IsValidLessonSubmissionPostRequest(submissionId);
        var handler = new IsValidLessonSubmissionPostHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}