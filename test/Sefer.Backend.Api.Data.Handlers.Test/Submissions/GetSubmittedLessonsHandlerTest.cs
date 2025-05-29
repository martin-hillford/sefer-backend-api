namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmittedLessonsHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        await base.Initialize();

        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var submission = new LessonSubmission { IsFinal = true, EnrollmentId = enrollment.Id, LessonId = lesson.Id };
        await InsertAsync(submission);
    }

    [TestMethod]
    public async Task Handle_TopTooLow()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var result = await Handle(student.Id, 0);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle_SubmissionNotFinal()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var submission = context.LessonSubmissions.Single();
        submission.IsFinal = false;
        context.Update(submission);
        await context.SaveChangesAsync();

        var result = await Handle(student.Id, 1);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle_NoStudent()
    {
        var result = await Handle(13, 0);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        var result = await Handle(student.Id, 1);
        Assert.AreEqual(1, result);
    }

    private async Task<int> Handle(int studentId, uint top)
    {
        var request = new GetSubmittedLessonsRequest(studentId, top);
        var provider = GetServiceProvider();
        var handler = new GetSubmittedLessonsHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        return result.Count;
    }
}