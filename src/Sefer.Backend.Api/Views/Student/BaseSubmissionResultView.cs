using Sefer.Backend.Api.Views.Shared.Courses.Surveys;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// The lesson submitted view deals with the result to return when a lesson is submitted (or saved)
/// </summary>
public class BaseSubmissionResultView
{
    #region Properties

    /// <summary>
    /// Gets/sets if the course is a self study course
    /// </summary>
    /// <returns></returns>
    public readonly bool IsSelfStudy;

    /// <summary>
    /// Holds if the results of the submission should be visible for the students
    /// </summary>
    public bool ResultsStudentVisible { get; init; }

    /// <summary>
    /// Gets/sets the id of the submission that is saved
    /// </summary>
    /// <returns></returns>
    public int SubmissionId { get; init; }

    /// <summary>
    /// Gets/sets the survey that belongs to this course (optional)
    /// </summary>
    /// <returns></returns>
    public SurveyView Survey { get; set; }

    /// <summary>
    /// The id of the enrollment
    /// </summary>
    public readonly int EnrollmentId;

    /// <summary>
    /// Contains a list of grants award because of this submission
    /// </summary>
    /// <value></value>
    public List<GrantView> Grants { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new base submission
    /// </summary>
    /// <param name="enrollment"></param>
    public BaseSubmissionResultView(Enrollment enrollment)
    {
        IsSelfStudy = enrollment.IsSelfStudy;
        EnrollmentId = enrollment.Id;
    }

    /// <summary>
    /// Creates a new base submission
    /// </summary>
    /// <param name="isSelfStudy"></param>
    /// <param name="enrollmentId"></param>
    public BaseSubmissionResultView(bool isSelfStudy, int enrollmentId)
    {
        IsSelfStudy = isSelfStudy;
        EnrollmentId = enrollmentId;
    }

    #endregion
}