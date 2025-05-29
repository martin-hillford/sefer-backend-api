namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class UpdateSingleSubmissionPropertyHandlerTest
    : UpdateSingleEntityPropertyHandlerTest<UpdateSingleSubmissionPropertyRequest, UpdateSingleSubmissionPropertyHandler, LessonSubmission>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await SubmissionUnitTest.Initialize(context);
    }

    protected override async Task<List<(LessonSubmission entity, string property, object newValue, bool updated)>> GetTestData()
    {
        var context = GetDataContext();
        var enrollment = context.Enrollments.Single();
        var lesson = context.Lessons.Single();

        var existing = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(existing);
        existing.IsFinal = true;

        return
        [
            (existing, "IsFinal", true, true),
            (new LessonSubmission(), "IsFinal", false, false)
        ];
    }
}