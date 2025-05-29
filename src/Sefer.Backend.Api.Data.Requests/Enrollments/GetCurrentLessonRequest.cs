namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetCurrentLessonRequest : IRequest<(Lesson,LessonSubmission, Enrollment)>
{
    public readonly int StudentId;

    public GetCurrentLessonRequest(int studentId)
    {
        StudentId = studentId;
    }
}