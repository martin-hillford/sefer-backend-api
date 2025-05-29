namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetNumberOfSubmittedLessonsTodayHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        await base.Initialize();
        var context = GetDataContext();

        var student1 = context.Users.Single(u => u.Role == UserRoles.Student);
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var courseRevision = context.CourseRevisions.Single();
        var lesson = context.Lessons.Single();

        var student2 = new User { Name = "student2", Role = UserRoles.Student, Gender = Genders.Male, Email = "student2@e.tld", YearOfBirth = 1987 };
        await InsertAsync(student2);

        var enrollment1 = new Enrollment { MentorId = mentor.Id, CourseRevisionId = courseRevision.Id, StudentId = student1.Id };
        var enrollment2 = new Enrollment { MentorId = mentor.Id, CourseRevisionId = courseRevision.Id, StudentId = student2.Id };
        await InsertAsync(enrollment1, enrollment2);

        await InsertAsync(new LessonSubmission { EnrollmentId = enrollment1.Id, IsFinal = true, LessonId = lesson.Id, SubmissionDate = DateTime.UtcNow });
        await InsertAsync(new LessonSubmission { EnrollmentId = enrollment2.Id, IsFinal = true, LessonId = lesson.Id, SubmissionDate = DateTime.UtcNow.AddDays(-2) });
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var student1 = context.Users.Single(u => u.Name == "student");
        await Handle(student1.Id, 1);
    }

    [TestMethod]
    public async Task Handle_NotToday()
    {
        var context = GetDataContext();
        var student2 = context.Users.Single(u => u.Name == "student2");
        await Handle(student2.Id, 0);
    }

    private async Task Handle(int studentId, int expected)
    {
        var request = new GetNumberOfSubmittedLessonsTodayRequest(studentId);
        var handler = new GetNumberOfSubmittedLessonsTodayHandler(ServiceProvider);
        var count = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, count);
    }
}