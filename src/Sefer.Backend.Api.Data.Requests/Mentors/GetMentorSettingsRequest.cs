namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorSettingsRequest : IRequest<MentorSettings>
{
    public int MentorId { get; init; }
}