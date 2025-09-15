// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Views.Student;

public class BaseEnrollmentView(Enrollment enrollment)
{
    public int CourseId => enrollment.CourseRevision.CourseId;
    
    public int StudentId => enrollment.StudentId;
    
    public int? MentorId => enrollment.MentorId;
    
    public int CourseRevisionId => enrollment.CourseRevisionId;
    
    public long CreationDate => enrollment.CreationDate.ToUnixTimes();
    
    public long? ClosureDate => enrollment.ClosureDate?.ToUnixTimes();
    
    public bool IsCourseCompleted => enrollment.IsCourseCompleted;
    
    public double? Grade => enrollment.Grade;

    public int? LastSubmittedLessonId => enrollment.LessonSubmissions?
        .Where(l => l.IsFinal)
        .OrderByDescending(l => l.SubmissionDate)
        .FirstOrDefault()?.LessonId;
}