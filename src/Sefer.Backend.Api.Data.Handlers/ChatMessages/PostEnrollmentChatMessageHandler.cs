namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostEnrollmentChatMessageHandler(IServiceProvider serviceProvider)
    : Handler<PostEnrollmentChatMessageRequest, Message>(serviceProvider)
{
    public override async Task<Message> Handle(PostEnrollmentChatMessageRequest request, CancellationToken token)
    {
        var enrollment = await Send(new GetEnrollmentByIdExtensivelyRequest(request.EnrollmentId), token);
        if (enrollment?.MentorId == null || enrollment.IsSelfStudy) return null;

        var channelRequest = new GetPersonalChannelRequest(enrollment.MentorId.Value, enrollment.StudentId);
        var channel = await Send(channelRequest, token);
        if (channel == null) return null;

        var content = new EnrollmentView(enrollment);
        var postMessageRequest = new PostObjectChatMessageRequest
        {
            ReferenceId = enrollment.Id,
            Message = content,
            Type = MessageTypes.StudentEnrollment,
            ChannelId = channel.Id,
            SenderId = enrollment.StudentId
        };
        return await Send(postMessageRequest, token);
    }
}