namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class SearchSubmissionRequest(int enrollmentId, int lessonId) : IRequest<LessonSubmission> 
{
    public int EnrollmentId => enrollmentId;
    
    public int LessonId => lessonId;
}