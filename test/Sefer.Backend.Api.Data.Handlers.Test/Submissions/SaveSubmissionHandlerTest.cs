namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class SaveSubmissionHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle_Invalid()
    {
        var provider = GetServiceProvider();
        var request = new SaveSubmissionRequest(null, []);
        await Handle(provider, request, false);
    }

    [TestMethod]
    public async Task Handle_Update()
    {
        await Handle_Insert(false, null);
        var context = GetDataContext();

        var submission = await context.LessonSubmissions.SingleAsync();
        submission.CurrentPage = 1;

        var provider = GetServiceProvider();
        var request = new SaveSubmissionRequest(submission, []);
        await Handle(provider, request, true);
    }

    [TestMethod]
    public async Task Handle_Update_WithAnswer()
    {
        await Handle_Insert(false, null);
        var context = GetDataContext();

        var submission = await context.LessonSubmissions.SingleAsync();
        submission.CurrentPage = 1;

        var answer = await context.Answers.SingleAsync();
        var answers = new List<QuestionAnswer> { answer };

        var provider = GetServiceProvider();
        var request = new SaveSubmissionRequest(submission, answers);
        await Handle(provider, request, true);
    }

    [TestMethod]
    public async Task Handle_Insert_NoAnswer()
    {
        var context = GetDataContext();
        var enrollment = await context.Enrollments.SingleAsync();
        var lesson = await context.Lessons.SingleAsync();

        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = false,
            LessonId = lesson.Id
        };

        var provider = GetServiceProvider();
        var request = new SaveSubmissionRequest(submission, []);
        await Handle(provider, request, true);
    }

    [TestMethod]
    public async Task Handle_Insert_Exception()
    {
        var context = GetDataContext();
        var enrollment = await context.Enrollments.SingleAsync();
        var lesson = await context.Lessons.SingleAsync();

        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = false,
            LessonId = lesson.Id
        };

        var provider = GetServiceProvider();
        var request = new SaveSubmissionRequest(submission, null);
        await Handle(provider, request, false);
    }

    [TestMethod]
    [DataRow(true, 1d)]
    [DataRow(false, null)]
    public async Task Handle_Insert(bool final, double? expectedGrade)
    {
        var context = GetDataContext();
        var enrollment = await context.Enrollments.SingleAsync();
        var lesson = await context.Lessons.SingleAsync();


        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = final,
            LessonId = lesson.Id
        };

        await InsertAsync(submission);
        Assert.AreEqual(1, context.LessonSubmissions.Count());

        var answer = await PrepareQuestion();
        var answers = new List<QuestionAnswer> { answer };

        var provider = GetServiceProvider();
        provider.AddRequestResult<CalculateSubmissionGradeRequest, double?>(expectedGrade);
        provider.AddRequestResult<GetSubmissionByIdRequest, LessonSubmission>(submission);
        provider.AddRequestResult<GetLessonIncludeReferencesRequest, Lesson>(lesson);
        var request = new SaveSubmissionRequest(submission, answers);
        await Handle(provider, request, true);

        Assert.AreEqual(1, context.Answers.Count());
        var inserted = await context.LessonSubmissions.SingleAsync();

        Assert.AreEqual(expectedGrade, inserted.Grade);
    }

    [TestMethod]
    public async Task Handle_CalculationException()
    {
        var context = GetDataContext();
        var enrollment = await context.Enrollments.SingleAsync();
        var lesson = await context.Lessons.SingleAsync();


        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = true,
            LessonId = lesson.Id
        };

        var answer = await PrepareQuestion();
        var answers = new List<QuestionAnswer> { answer };

        var provider = GetServiceProvider();
        provider.AddRequestException<CalculateSubmissionGradeRequest, double?>();
        var request = new SaveSubmissionRequest(submission, answers);
        await Handle(provider, request, false);
    }

    private async Task<QuestionAnswer> PrepareQuestion()
    {
        var context = GetDataContext();
        var lesson = await context.Lessons.SingleAsync();
        return await PrepareQuestion(context, lesson);
    }

    private static async Task Handle(MockedServiceProvider provider, SaveSubmissionRequest request, bool expected)
    {
        var handler = new SaveSubmissionHandler(provider.Object);
        var response = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, response);
    }

}