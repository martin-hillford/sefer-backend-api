namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class SaveMentorStudentDataRequest(MentorStudentData data) : IRequest<bool>
{
    public readonly MentorStudentData Data = data;
}