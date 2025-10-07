namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionsByTimeRequest(int studentId, DateTime? start) : IRequest<List<LessonSubmission>>
{
    public DateTime? Start => start;
    
    public int StudentId => studentId;
}