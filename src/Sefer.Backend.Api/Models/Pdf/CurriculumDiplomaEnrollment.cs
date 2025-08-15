namespace Sefer.Backend.Api.Models.Pdf;

public class CurriculumDiplomaEnrollment(Enrollment enrollment)
{
    /// <summary>
    /// This of the course
    /// </summary>
    public string CourseName => enrollment.CourseRevision.Course.Name;
    
    /// <summary>
    /// A rounded display value of the grade
    /// </summary>
    public double? GradeRounded => enrollment.GradeRounded;
    
    /// <summary>
    /// This contains the final grade for of the course (between 0 and zero)
    /// </summary>
    public double? Grade => enrollment.Grade;
}