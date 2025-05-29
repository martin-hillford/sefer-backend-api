namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetNumberOfSubmittedLessonsTodayRequest(int studentId) : IRequest<int>
{
    public readonly int StudentId = studentId;
}