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
    /// <remarks></remarks>
    public string GradeRounded => enrollment.GradeRounded.HasValue
        ? (enrollment.GradeRounded.Value * 10).ToString("0.0", CultureInfo.InvariantCulture)
        : string.Empty;
        
    /// <summary>
    /// This contains the final grade for of the course (between 0 and zero)
    /// </summary>
    public double? Grade => enrollment.Grade;
}