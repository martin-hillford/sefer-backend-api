namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetPreviousFeedbackRequest(int mentorId, int courseRevisionId) : IRequest<List<QuestionAnswer>>
{
    public readonly int MentorId = mentorId;

    public readonly int CourseRevisionId = courseRevisionId;
}

