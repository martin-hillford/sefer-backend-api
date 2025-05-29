namespace Sefer.Backend.Api.Data.Requests.Channels;

/// <summary>
/// Loads for the given mentor the activity of the students
/// </summary>
public class GetActiveChannelsRequest(int mentorId) : IRequest<Dictionary<int, UserLastActivity>>
{
    public readonly int MentorId = mentorId;
}