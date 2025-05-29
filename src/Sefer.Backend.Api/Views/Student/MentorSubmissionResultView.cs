namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// The lesson submitted view deals with the result to return when a lesson is submitted (or saved)
/// </summary>
/// <inheritdoc />
public class MentorSubmissionResultView : BaseSubmissionResultView
{
    #region Constructor

    /// <summary>
    /// Creates a new mentor submission result
    /// </summary>
    /// <returns></returns>
    /// <inheritdoc />
    public MentorSubmissionResultView(int enrollmentId) : base(false,enrollmentId) { }

    #endregion
}