namespace Sefer.Backend.Api.Data.Handlers.Test.ChatMessages;

[TestClass]
public class PostSubmissionMessageHandlerTest : PostChatMessageHandlerTest
{
    [TestMethod]
    public async Task Handle_InvalidSubmission()
    {
        var message = await Handle(13, false, null, null);
        message.Should().BeNull();
    }
    
    [TestMethod]
    public async Task Handle_SubmissionNull()
    {
        var message = await Handle(13, true, null, null);
        message.Should().BeNull();  
    }
    
    [TestMethod]
    public async Task Handle_SubmissionResultsVisible()
    {
        var submission = new LessonSubmission { ResultsStudentVisible = true };
        var message = await Handle(13, true, submission, null);
        message.Should().BeNull();  
    }
    
    [TestMethod]
    public async Task Handle_EnrollmentNull()
    {
        var submission = new LessonSubmission { ResultsStudentVisible = false };
        var message = await Handle(13, true, submission, null);
        message.Should().BeNull();  
    }
    
    [TestMethod]
    public async Task Handle_ChannelNull()
    {
        var enrollment = new Enrollment();
        var submission = new LessonSubmission { ResultsStudentVisible = false, Enrollment = enrollment };
        var message = await Handle(13, true, submission, null);
        message.Should().BeNull();     
    }

    [TestMethod]
    public async Task Handle()
    {
        var (channel, submission) = await InitializeSubmission(false);
        var message = Handle(submission.Id, true, submission, channel);
        message.Should().NotBeNull();
    }

    private async Task<Message> Handle(int submissionId, bool isValidSubmissionPost, LessonSubmission? submission, Channel? channel)
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<IsValidLessonSubmissionPostRequest, bool>(isValidSubmissionPost);
        if(submission != null) provider.AddRequestResult<GetSubmissionWithEnrollmentByIdRequest, LessonSubmission>(submission);
        if(channel != null) provider.AddRequestResult<GetPersonalChannelRequest, Channel>(channel);
        provider.AddRequestResult<PostObjectChatMessageRequest, Message>(new Message());
        if(submission != null) provider.AddRequestResult<GetFinalSubmissionByIdRequest, LessonSubmission>(submission);
        var request = new PostSubmissionMessageRequest(submissionId);
        var handler = new PostSubmissionMessageHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}