namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostSubmissionMessageHandler(IServiceProvider serviceProvider)
    : PostChatMessageHandler<PostSubmissionMessageRequest, Message>(serviceProvider)
{
    public override async Task<Message> Handle(PostSubmissionMessageRequest request, CancellationToken token)
    {
        // Check everything before posting the message
        var isValid = await Send(new IsValidLessonSubmissionPostRequest(request.LessonSubmissionId), token);
        var submission = await Send(new GetSubmissionWithEnrollmentByIdRequest(request.LessonSubmissionId), token);
        if (!isValid || submission?.ResultsStudentVisible != false || submission.Enrollment == null) return null;

        // Get the channel to post the message in
        var channel = await Send(new GetPersonalChannelRequest(submission.Enrollment), token);
        if (channel == null) return null;

        // and return the result of posting the message
        var objectView = await CreateLessonSubmissionView(submission.Id, token);
        var postMessageRequest = new PostObjectChatMessageRequest
        {
            Message = objectView,
            Type = MessageTypes.StudentLessonSubmission,
            ReferenceId = submission.Id,
            SenderId = submission.Enrollment.StudentId,
            ChannelId = channel.Id
        };
        return await Send(postMessageRequest, token);
    }
}