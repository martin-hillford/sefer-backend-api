namespace Sefer.Backend.Api.Data.Requests.Channels;

public class DeletePrivateChannelRequest(int mentorId, int channelId) : IRequest<bool>
{
    public readonly int MentorId = mentorId;
    
    public readonly int ChannelId = channelId;
}