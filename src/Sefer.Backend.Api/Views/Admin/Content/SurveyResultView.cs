// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Content;

/// <summary>
/// This view will be used to return a survey result to the admin
/// </summary>
public class SurveyResultView : AbstractView<SurveyResult>
{
    /// <summary>
    /// The mentor that was involved in the course
    /// </summary>
    public UserView Mentor { get; private set; }

    /// <summary>
    /// The student that toke the course
    /// </summary>
    public UserView Student { get; private set; }

    /// <summary>
    /// The course the enrollment was about
    /// </summary>
    public CourseView Course { get; private set; }

    /// <summary>
    /// The rating for the mentor (if applicable)
    /// </summary>
    public byte? MentorRating => Model?.MentorRating?.Rating;

    /// <summary>
    /// The rating for the mentor (if applicable)
    /// </summary>
    public byte? CourseRating => Model?.CourseRating?.Rating;

    /// <summary>
    /// The testimony of the student about the course
    /// </summary>
    public string Text => Model?.Text;

    /// <summary>
    /// Holds if the student gives permission to use his testimony on social media
    /// </summary>
    public bool SocialPermissions => Model.SocialPermissions;

    /// <summary>
    /// Holds the student this the study as self study
    /// </summary>
    public bool? SelfStudy => Model.Enrollment?.IsSelfStudy;

    /// <summary>
    /// Holds if the admin has processed the survey result
    /// </summary>
    public bool AdminProcessed => Model.AdminProcessed;

    /// <summary>
    /// Returns the date the enrolment was closed
    /// </summary>
    public DateTime? ClosureDate
    {
        get
        {
            if (Model?.Enrollment?.IsCourseCompleted != true) return null;
            return Model?.Enrollment?.ClosureDate;
        }
    }

    /// <summary>
    /// Creates a new view
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public SurveyResultView(SurveyResult result) : base(result)
    {
        if (result?.Enrollment?.CourseRevision?.Course != null) Course = new CourseView(result.Enrollment.CourseRevision.Course);
        if (result?.Enrollment?.Mentor != null) Mentor = new UserView(result.Enrollment.Mentor);
        if (result?.Enrollment?.Student != null) Student = new UserView(result.Enrollment.Student);
    }
}