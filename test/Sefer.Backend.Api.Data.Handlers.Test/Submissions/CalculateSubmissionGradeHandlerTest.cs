namespace Sefer.Backend.Api.Data.Handlers.Test.Submissions;

[TestClass]
public class CalculateSubmissionGradeHandlerTest : SubmissionUnitTest
{
    [TestMethod]
    public async Task Handle_No_Submission()
    {
        await Assert.ThrowsExactlyAsync<NullReferenceException>(async () => await Handle(19, GetServiceProvider()));
    }

    [TestMethod]
    public async Task Handle_No_Enrollment()
    {
        var submission = new LessonSubmission();
        var provider = new MockedServiceProvider();
        provider.AddRequestResult<GetSubmissionByIdRequest, LessonSubmission>(submission);
        await Assert.ThrowsExactlyAsync<NullReferenceException>(async () => await Handle(submission.Id, provider));
    }

    [TestMethod]
    public async Task Handle_No_Lesson()
    {
        var submission = new LessonSubmission();
        var enrollment = new Enrollment();
        var provider = new MockedServiceProvider();
        provider.AddRequestResult<GetSubmissionByIdRequest, LessonSubmission>(submission);
        provider.AddRequestResult<GetEnrollmentByIdRequest, Enrollment>(enrollment);
        await Assert.ThrowsExactlyAsync<NullReferenceException>(async () => await Handle(submission.Id, provider));
    }

    [TestMethod]
    public async Task Handle_No_Answers()
    {
        var pairs = new List<(Question, QuestionAnswer?)> { (new BoolQuestion(), null) };
        var grade = await Handle(pairs);
        Assert.IsNull(grade);
    }

    [TestMethod]
    public async Task Handle_Only_OpenQuestion()
    {
        var pairs = new List<(Question, QuestionAnswer?)> { (new OpenQuestion(), new QuestionAnswer()) };
        var grade = await Handle(pairs);
        Assert.AreEqual(1, grade);
    }

    [TestMethod]
    public async Task Handle_WrongBoolQuestion()
    {
        var pairs = new List<(Question, QuestionAnswer?)>
        {
            (
                new BoolQuestion { CorrectAnswer = BoolAnswers.Correct },
                new QuestionAnswer  { TextAnswer = "Wrong", QuestionType = ContentBlockTypes.QuestionBoolean }
            ),
            (
                new BoolQuestion { CorrectAnswer = BoolAnswers.Wrong },
                new QuestionAnswer  { TextAnswer = "Wrong", QuestionType = ContentBlockTypes.QuestionBoolean }
            ),
        };

        var grade = await Handle(pairs);
        Assert.AreEqual(0.5d, grade);
    }

    [TestMethod]
    [DataRow("17,13", true)]
    [DataRow("13,17", true)]
    [DataRow("13,19", false)]
    [DataRow("13", false)]
    [DataRow("17", false)]
    public void IsCorrectMultipleChoiceQuestion(string textAnswer, bool correct)
    {
        var choices = new List<MultipleChoiceQuestionChoice>
        {
            new() {Id = 13, IsCorrectAnswer = true},
            new() {Id = 17, IsCorrectAnswer = true},
            new() {Id = 19, IsCorrectAnswer = false},
        };
        var question = new MultipleChoiceQuestion { Choices = choices, IsMultiSelect = true };
        var answer = new QuestionAnswer { TextAnswer = textAnswer };

        var result = answer.IsCorrectMultipleChoiceQuestion(question);
        Assert.AreEqual(correct, result);
    }

    [TestMethod]
    public void IsCorrectMultipleChoiceQuestion_Exception()
    {
        var question = new BoolQuestion();
        var answer = new QuestionAnswer();

        var result = answer.IsCorrectMultipleChoiceQuestion(question);
        Assert.IsFalse(result);
    }

    private async Task<double?> Handle(List<(Question Question, QuestionAnswer? Answer)> pairs)
    {
        var context = GetDataContext();
        var lesson = context.Lessons.Single();
        var enrollment = context.Enrollments.Single();
        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = false, LessonId = lesson.Id, };
        await InsertAsync(submission);

        lesson.Content = pairs.Select(c => c.Question).Cast<IContentBlock<Lesson>>();

        foreach (var pair in pairs)
        {
            pair.Question.LessonId = lesson.Id;
            pair.Question.Content = "test";
            await InsertAsync(pair.Question);

            if (pair.Answer == null) continue;
            pair.Answer.QuestionId = pair.Question.Id;
            pair.Answer.SubmissionId = submission.Id;
            await InsertAsync(pair.Answer);
        }

        var provider = GetServiceProvider();
        provider
            .AddRequestResult<GetSubmissionByIdRequest, LessonSubmission>(submission)
            .AddRequestResult<GetEnrollmentByIdRequest, Enrollment>(enrollment)
            .AddRequestResult<GetLessonIncludeReferencesRequest, Lesson>(lesson);

        return await Handle(submission.Id, provider);
    }

    private static async Task<double?> Handle(int submissionId, MockedServiceProvider provider)
    {
        var request = new CalculateSubmissionGradeRequest(submissionId);
        var handler = new CalculateSubmissionGradeHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}