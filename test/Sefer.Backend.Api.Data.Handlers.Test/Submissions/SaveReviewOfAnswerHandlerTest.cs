namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class SaveReviewOfAnswerHandlerTest : SubmissionUnitTest
{
    [TestInitialize]
    public override async Task Initialize()
    {
        await base.Initialize();

        var context = GetDataContext();
        var lesson = await context.Lessons.SingleAsync();
        var enrollment = await context.Enrollments.SingleAsync();

        var question = new BoolQuestion
        {
            Content = "This question is true",
            CorrectAnswer = BoolAnswers.Correct,
            LessonId = lesson.Id
        };
        await InsertAsync(question);

        var submission = new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            CurrentPage = 0,
            IsFinal = true,
            LessonId = lesson.Id,
            SubmissionDate = DateTime.Today
        };
        await InsertAsync(submission);

        var answer = new QuestionAnswer
        {
            QuestionId = question.Id,
            QuestionType = ContentBlockTypes.QuestionBoolean,
            TextAnswer = "correct",
            SubmissionId = submission.Id
        };

        await InsertAsync(answer);
    }

    [TestMethod]
    public async Task Handle_NoAnswer()
    {
        var result = await Handle(29, "review");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var answer = context.Answers.Single();
        await context.DisposeAsync();
        var result = await Handle(answer.Id, "review");

        Assert.IsTrue(result);

        await using var dataContext = GetDataContext();
        var saved = dataContext.Answers.Single();
        Assert.AreEqual("review", saved.MentorReview);
    }

    private async Task<bool> Handle(int answerId, string review)
    {
        var provider = GetServiceProvider();
        var request = new SaveReviewOfAnswerRequest(answerId, review);
        var handler = new SaveReviewOfAnswerHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}