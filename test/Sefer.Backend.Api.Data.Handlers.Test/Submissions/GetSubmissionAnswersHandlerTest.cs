namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class GetSubmissionAnswersHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();

        var lesson = context.Lessons.Single();
        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var courseRevision = context.CourseRevisions.Single();
        var enrollmentA = context.Enrollments.Single();
        var question = new OpenQuestion { LessonId = lesson.Id, Content = "question" };
        await InsertAsync(question);

        var submissionA = new LessonSubmission { EnrollmentId = enrollmentA.Id, IsFinal = false, LessonId = lesson.Id, };
        var userB = new User { Name = "student", Role = UserRoles.Student, Gender = Genders.Male, Email = "student@ea.tld", YearOfBirth = 1987 };
        await InsertAsync(userB);
        var enrollmentB = new Enrollment { MentorId = mentor.Id, CourseRevisionId = courseRevision.Id, StudentId = userB.Id };
        await InsertAsync(enrollmentB);
        var submissionB = new LessonSubmission { EnrollmentId = enrollmentB.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submissionA, submissionB);

        var answerA = new QuestionAnswer { QuestionId = question.Id, SubmissionId = submissionA.Id };
        var answerB = new QuestionAnswer { QuestionId = question.Id, SubmissionId = submissionB.Id };
        await InsertAsync(answerA, answerB);

        var provider = GetServiceProvider();
        var handler = new GetSubmissionAnswersHandler(provider.Object);

        var resultA = await handler.Handle(new GetSubmissionAnswersRequest(submissionA.Id), CancellationToken.None);
        var resultB = await handler.Handle(new GetSubmissionAnswersRequest(submissionB.Id), CancellationToken.None);

        Assert.AreEqual(1, resultA.Count);
        Assert.AreEqual(1, resultB.Count);
        Assert.AreEqual(resultA.First().Id, answerA.Id);
        Assert.AreEqual(resultB.First().Id, answerB.Id);
        Assert.AreNotEqual(resultA.First().Id, answerB.Id);
        Assert.AreNotEqual(resultB.First().Id, answerA.Id);
    }
}