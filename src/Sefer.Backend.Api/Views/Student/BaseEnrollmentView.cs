// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Views.Student;

public class BaseEnrollmentView(Enrollment enrollment)
{
    public int CourseId => enrollment.CourseRevision.CourseId;
    
    public int StudentId => enrollment.StudentId;
    
    public int? MentorId => enrollment.MentorId;
    
    public int CourseRevisionId => enrollment.CourseRevisionId;
    
    public DateTime CreationDate => enrollment.CreationDate;
    
    public DateTime? ClosureDate => enrollment.ClosureDate;
    
    public bool IsCourseCompleted => enrollment.IsCourseCompleted;
    
    public double? Grade => enrollment.Grade;

    public int? LastSubmittedLessonId => enrollment.LessonSubmissions?
        .Where(l => l.IsFinal)
        .OrderByDescending(l => l.SubmissionDate)
        .FirstOrDefault()?.LessonId;
}