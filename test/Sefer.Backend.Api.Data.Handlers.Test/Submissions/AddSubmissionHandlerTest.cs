namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class AddSubmissionHandlerTest : AddEntityHandlerTest<AddSubmissionRequest, AddSubmissionHandler, LessonSubmission>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await SubmissionUnitTest.Initialize(context);
    }

    protected override List<(LessonSubmission Entity, bool Valid)> GetTestData()
    {
        var context = GetDataContext();
        var lesson = context.Lessons.First();
        var enrollment = context.Lessons.First();

        return
        [
            (new LessonSubmission(), false),
            (new LessonSubmission { EnrollmentId = 19, LessonId = 21, IsFinal = false }, false),
            (new LessonSubmission { EnrollmentId = enrollment.Id, LessonId = 21, IsFinal = false }, false),
            (new LessonSubmission { EnrollmentId = 19, LessonId = lesson.Id, IsFinal = false }, false),
            (new LessonSubmission { EnrollmentId = enrollment.Id, LessonId = lesson.Id, IsFinal = true }, true),
            (new LessonSubmission { EnrollmentId = enrollment.Id, LessonId = lesson.Id, IsFinal = false }, true),
        ];
    }
}