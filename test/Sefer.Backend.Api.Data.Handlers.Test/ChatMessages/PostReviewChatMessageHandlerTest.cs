namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostReviewChatMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_InValid()
    {
        var messages = await Handle(13, false, null, null);
        Assert.IsNull(messages);
    }

    [TestMethod]
    public async Task Handle_NoObjectView()
    {
        var messages = await Handle(13, true, null, null);
        Assert.IsNull(messages);
    }

    [TestMethod]
    public async Task Handle_NoChannel()
    {
        var (_, submission) = await InitializeSubmission(true);
        var messages = await Handle(submission.Id, true, submission, null);
        Assert.IsNull(messages);
        
    }

    [TestMethod]
    public async Task Handle_WithoutMessageReview()
    {
        var (channel, submission) = await InitializeSubmission(true);
        var messages = await Handle(submission.Id, true, submission, channel);
        Assert.IsNotNull(messages);
        Assert.AreEqual(1, messages.Count);
    }

    [TestMethod]
    public async Task Handle_WithMessageReview()
    {
        var (channel, submission) = await InitializeSubmission(true, "review");
        var messages = await Handle(submission.Id, true, submission, channel);
        messages.Should().NotBeNull();
        messages.Count.Should().Be(2);
    }

    [TestMethod]
    public async Task Handle_NoReviewView()
    {
        var (channel, submission) = await InitializeSubmission(true, "review");
        submission.Answers.First().QuestionType = ContentBlockTypes.QuestionMultipleChoice;
        var messages = await Handle(submission.Id, true, submission, channel);
        messages.Should().NotBeNull();
        messages.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle_NoMentorId()
    {
        var (channel, submission) = await InitializeSubmission(true, "review");
        submission.Enrollment.MentorId = null;
        var messages = await Handle(submission.Id, true, submission, channel);
        Assert.IsNull(messages);
    }

    [TestMethod]
    public async Task Handle_OpenQuestion()
    {
        var (channel, submission) = await InitializeSubmission(true, "review");

        var context = GetDataContext();
        var lesson = context.Lessons.Single();

        var openQuestion = new OpenQuestion { Content = "content", LessonId = lesson.Id, IsMarkDownContent = false, Heading = "1", Number = "1" };
        await InsertAsync(openQuestion);
        submission.Lesson.Content = new List<IContentBlock<Lesson>>(submission.Lesson.Content) { openQuestion };

        var answer = new QuestionAnswer { QuestionId = openQuestion.Id, QuestionType = ContentBlockTypes.QuestionOpen, TextAnswer = "test", SubmissionId = submission.Id, MentorReview = "review" };
        await InsertAsync(answer);
        submission.Answers.Add(answer);

        var messages = await Handle(submission.Id, true, submission, channel);
        messages.Should().NotBeNull();
        messages.Count.Should().Be(3);
    }

    [TestMethod]
    public async Task Handle_MultipleChoiceQuestion()
    {
        var (channel, submission) = await InitializeSubmission(true, "review");

        var context = GetDataContext();
        var lesson = context.Lessons.Single();

        var question = new MultipleChoiceQuestion { Content = "content", LessonId = lesson.Id, IsMarkDownContent = false, Heading = "1", Number = "1" };
        await InsertAsync(question);
        var option = new MultipleChoiceQuestionChoice { QuestionId = question.Id, Answer = "answer", IsCorrectAnswer = true };
        await InsertAsync(option);
        submission.Lesson.Content = new List<IContentBlock<Lesson>>(submission.Lesson.Content) { question };

        var answer = new QuestionAnswer { QuestionId = question.Id, QuestionType = ContentBlockTypes.QuestionMultipleChoice, TextAnswer = $"{option.Id}", SubmissionId = submission.Id, MentorReview = "review" };
        await InsertAsync(answer);
        submission.Answers.Add(answer);

        var messages = await Handle(submission.Id, true, submission, channel);
        messages.Should().NotBeNull();
        messages.Count.Should().Be(3);
    }

    private async Task<List<Message>> Handle(int submissionId, bool isValidSubmissionPost, LessonSubmission? submission, Channel? channel)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsValidLessonSubmissionPostRequest, bool>(isValidSubmissionPost);
        if(channel != null) provider.AddRequestResult<GetPersonalChannelRequest, Channel>(channel);
        provider.AddRequestResult<PostObjectChatMessageRequest, Message>(new Message());
        if(submission != null) provider.AddRequestResult<GetFinalSubmissionByIdRequest, LessonSubmission>(submission);

        var request = new PostReviewChatMessageRequest(submissionId);
        var handler = new PostReviewChatMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}