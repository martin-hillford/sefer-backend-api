namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmittedLessonsBetweenDatesCountHandlerTest : SubmissionUnitTest
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
    public async Task Handle_NoStudent()
    {
        var result = await Handle(DateTime.MinValue, DateTime.MaxValue, 13);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task NoSubmissionDate()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();
        submission.SubmissionDate = null;
        context.Update(submission);
        await context.SaveChangesAsync();

        var result = await Handle(DateTime.MinValue, DateTime.MaxValue);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle_SubmissionNotFinal()
    {
        var context = GetDataContext();
        var submission = context.LessonSubmissions.Single();
        submission.IsFinal = false;
        context.Update(submission);
        await context.SaveChangesAsync();

        var result = await Handle(DateTime.MinValue, DateTime.MaxValue);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle_LowerBound()
    {
        var result = await Handle(DateTime.MinValue, DateTime.Now.AddDays(-2));
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle_UpperBound()
    {
        var result = await Handle(DateTime.Now.AddDays(1), DateTime.MaxValue);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var result = await Handle(DateTime.MinValue, DateTime.MaxValue);
        Assert.AreEqual(1, result);
    }

    private async Task<int> Handle(DateTime lower, DateTime upper)
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        return await Handle(lower, upper, student.Id);
    }

    private async Task<int> Handle(DateTime lower, DateTime upper, int studentId)
    {
        var request = new GetSubmittedLessonsBetweenDatesCountRequest(lower, upper, studentId);
        var provider = GetServiceProvider();
        var handler = new GetSubmittedLessonsBetweenDatesCountHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}