// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Requests.Students;

public class GetStudentTierRequest(int studentId) : IRequest<StudentTier>
{
    public readonly int StudentId = studentId;
    
    public DateTime StartDate { get; private set; } = DateTime.MinValue;

    public DateTime EndDate { get; private set; } = DateTime.MaxValue;

    public static GetStudentTierRequest ThisYear(int studentId)
    {
        var start = DateTime.UtcNow;
        var end = new DateTime(start.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return new GetStudentTierRequest(studentId) { StartDate = start, EndDate = end };
    }
    
    public static GetStudentTierRequest ThisMonth(int studentId)
    {
        var start = DateTime.UtcNow;
        var end = new DateTime(start.Year, start.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return new GetStudentTierRequest(studentId) { StartDate = start, EndDate = end };
    }

    public static GetStudentTierRequest Overall(int studentId) => new(studentId);
}

public class StudentTier
{
    public int CompletedLessonByStudentCount { get; init; }
    
    public int StudentCountWithMoreCompletedLessons { get; set; }
    
    public int StudentCountWithLessCompletedLessons { get; set; }
    
    public int StudentCountWithEqualCompletedLessons { get; set; }
    
    public int TotalStudentCount { get; init; }
    
    public double Tier =>
        CompletedLessonByStudentCount == 0 
            ? 100
            : (1- (TotalStudentCount - StudentCountWithMoreCompletedLessons) / (double)TotalStudentCount) * 100;
}