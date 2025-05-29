namespace Sefer.Backend.Api.Data.Requests.Channels;

public class UpdateChannelRequest(int channelId, int mentorId, string name, List<int> students) : IRequest<Channel>
{
    public readonly int ChannelId = channelId;
    
    public readonly int MentorId = mentorId;

    public readonly string Name = name;
    
    public readonly List<int> Students = students;
}