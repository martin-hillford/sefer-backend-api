namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostMentorChangeChatMessageHandler(IServiceProvider serviceProvider)
    : PostChatMessageHandler<PostMentorChangeChatMessageRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(PostMentorChangeChatMessageRequest request, CancellationToken token)
    {
        var messages = new List<Message>();

        var student = await Send(new GetUserByIdRequest(request.Student), token);
        var newMentor = await Send(new GetUserByIdRequest(request.NewMentor), token);
        var oldMentor = await Send(new GetUserByIdRequest(request.OldMentor), token);

        if (student == null || newMentor == null) return messages;
        if (student.IsMentor || !newMentor.IsMentor) return messages;
        if (oldMentor is null or { IsMentor: false }) return messages;

        var newMentorChannel = await Send(new GetPersonalChannelRequest(student.Id, newMentor.Id), token);
        if (newMentorChannel == null) return messages;

        var enterMessage = await Post(request.CourseName, student.Id, newMentorChannel.Id, MessageTypes.MentorChangeEnter, token);
        if (enterMessage != null) messages.Add(enterMessage);

        var oldMentorChannel = await Send(new GetPersonalChannelRequest(student.Id, oldMentor.Id), token);
        if (oldMentorChannel == null) return messages;

        var leaveMessage = await Post(request.CourseName, student.Id, oldMentorChannel.Id, MessageTypes.MentorChangeLeave, token);
        if (leaveMessage != null) messages.Add(leaveMessage);

        return messages;
    }
}