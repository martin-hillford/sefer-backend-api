namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmittedLessonsBetweenDatesCountRequest(DateTime lowerBound, DateTime upperBound, int studentId)
    : IRequest<int>
{
    public readonly DateTime LowerBound = lowerBound;

    public readonly DateTime UpperBound = upperBound;

    public readonly int StudentId = studentId;
}