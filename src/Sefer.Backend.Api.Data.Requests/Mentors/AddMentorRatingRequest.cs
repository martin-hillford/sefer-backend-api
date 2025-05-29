namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class AddMentorRatingRequest(MentorRating mentorRating) : IRequest<bool>
{
    public readonly MentorRating MentorRating = mentorRating;
}