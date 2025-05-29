namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class RemoveRegionsForMentorRequest(int mentorId, List<string> regions) : IRequest<bool>
{
    public readonly int MentorId = mentorId;

    public readonly ReadOnlyCollection<string> Regions = regions.AsReadOnly();
}