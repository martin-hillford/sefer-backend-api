namespace Sefer.Backend.Api.Views.App;

/// <summary>
/// An enrollment as send to the client
/// </summary>
/// <param name="enrollment"></param>
public class EnrollmentView(Enrollment enrollment)
{
    [JsonPropertyName("id")]
    public int Id => enrollment.Id;

    /// <summary>
    /// The date the enrollment was created.
    /// </summary>
    [JsonPropertyName("cr_dt")]
    public long CreationDate => enrollment.CreationDate.ToUnixTime();

    /// <summary>
    /// The date the enrollment was modified for the last time
    /// </summary>
    [JsonPropertyName("m_dt")]
    public long? ModificationDate => enrollment.ModificationDate?.ToUnixTime();
    
    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    [JsonPropertyName("cl_dt")]
    public long? ClosureDate => enrollment.ClosureDate?.ToUnixTime();
    
    /// <summary>
    /// The id of the revision of the course that is taken by the student.
    /// </summary>
    [JsonPropertyName("cr_id")]
    public int CourseRevisionId => enrollment.CourseRevisionId;
    
    /// <summary>
    /// The id of the student that is enrolled to the course.
    /// </summary>
    [JsonPropertyName("s_id")]
    public int StudentId => enrollment.StudentId;
    
    /// <summary>
    /// Holds if the User has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    [JsonPropertyName("dn")]
    public int IsCourseCompleted => enrollment.IsCourseCompleted ? 1 : 0;
    
    /// <summary>
    /// Gets the CourseId of this enrollment
    /// </summary>
    [JsonPropertyName("c_id")]
    public int CourseId => enrollment.CourseRevision.CourseId;
    
    /// <summary>
    /// This contains the final grade for of the course (between 0 and zero)
    /// </summary>
    /// <value></value>
    [JsonPropertyName("grd")]
    public double? Grade =>  enrollment.Grade;
}