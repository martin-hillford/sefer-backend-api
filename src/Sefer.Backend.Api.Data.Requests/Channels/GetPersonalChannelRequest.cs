namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetPersonalChannelRequest : IRequest<Channel>
{
    public readonly int UserA;

    public readonly int UserB;
    
    public GetPersonalChannelRequest(int userA, int userB)
    {
        UserA = userA;
        UserB = userB;
    }

    public GetPersonalChannelRequest(Enrollment enrollment)
    {
        if (!enrollment.MentorId.HasValue) return;
        UserA = enrollment.MentorId.Value;
        UserB = enrollment.StudentId;
    }
}