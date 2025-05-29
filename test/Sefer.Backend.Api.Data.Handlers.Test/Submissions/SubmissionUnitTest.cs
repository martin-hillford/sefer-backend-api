namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

public abstract class SubmissionUnitTest : HandlerUnitTest
{
    [TestInitialize]
    public virtual async Task Initialize()
    {
        var context = GetDataContext();
        await Initialize(context);
    }

    public static async Task Initialize(DataContext context)
    {
        await InitializeUsers(context);

        var mentor = context.Users.Single(u => u.Role == UserRoles.Mentor);
        var student = context.Users.Single(u => u.Role == UserRoles.Student);
        await InitializeEnrollment(context, student, mentor);
    }

    protected static async Task InitializeUsers(DataContext context)
    {
        var student = new User { Name = "student", Role = UserRoles.Student, Gender = Genders.Male, Email = "student@e.tld", YearOfBirth = 1987 };
        var mentor = new User { Name = "mentor", Role = UserRoles.Mentor, Gender = Genders.Male, Email = "mentor@e.tld", YearOfBirth = 1987 };
        await InsertAsync(context, student, mentor);
    }

    private static async Task InitializeEnrollment(DataContext context, IEntity? student = null, IEntity? mentor = null)
    {
        mentor ??= context.Users.Single(u => u.Role == UserRoles.Mentor);
        student ??= context.Users.Single(u => u.Role == UserRoles.Student);

        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(context, course);

        var courseRevision = new CourseRevision { CourseId = course.Id, Stage = Stages.Edit, Version = 1 };
        await InsertAsync(context, courseRevision);

        var lesson = new Lesson { Description = "lesson 1", Name = "lesson 1", CourseRevisionId = courseRevision.Id, Number = "1" };
        await InsertAsync(context, lesson);

        var enrollment = new Enrollment { MentorId = mentor.Id, CourseRevisionId = courseRevision.Id, StudentId = student.Id };
        await InsertAsync(context, enrollment);
    }

    public static async Task InitializeSubmission(DataContext context, IEntity? student = null, IEntity? mentor = null)
    {
        await InitializeEnrollment(context, student, mentor);

        var lesson = context.Lessons.Single();
        var enrollment = context.Enrollments.Single();

        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = true,
            LessonId = lesson.Id,
            Imported = false,
        };

        await InsertAsync(context, submission);
        var answer = await PrepareQuestion(context, lesson);
        answer.SubmissionId = submission.Id;
        await InsertAsync(context, answer);
    }
    protected static async Task<QuestionAnswer> PrepareQuestion(DataContext context, Lesson lesson)
    {
        var question = new BoolQuestion
        {
            Content = "This question is true",
            CorrectAnswer = BoolAnswers.Correct,
            LessonId = lesson.Id
        };
        await InsertAsync(context, question);

        return new QuestionAnswer
        {
            QuestionId = question.Id,
            QuestionType = ContentBlockTypes.QuestionBoolean,
            TextAnswer = "correct",
        };
    }
}