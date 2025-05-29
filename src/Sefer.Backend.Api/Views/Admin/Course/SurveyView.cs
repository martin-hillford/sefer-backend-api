using Sefer.Backend.Api.Data.Models.Courses.Surveys;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// A custom view on the survey with included revision versions and course information
/// </summary>
/// <inheritdoc cref="Shared.Courses.Surveys.SurveyView" />
public class SurveyView : Shared.Courses.Surveys.SurveyView
{
    #region Properties

    /// <summary>
    /// The revision of the survey
    /// </summary>
    private readonly CourseRevision _revision;

    /// <summary>
    /// The course of the survey
    /// </summary>
    private readonly Data.Models.Courses.Course _course;

    /// <summary>
    /// The id of the course of the survey
    /// </summary>
    public int CourseId => _course.Id;

    /// <summary>
    /// The name of the course of the survey
    /// </summary>
    public string CourseName => _course.Name;

    /// <summary>
    /// The version of the course revision of the survey
    /// </summary>
    public int CourseRevisionVersion => _revision.Version;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="revision">The revision of the survey</param>
    /// <param name="course">The course of the survey</param>
    /// <inheritdoc />
    public SurveyView(Survey model, CourseRevision revision, Data.Models.Courses.Course course) : base(model)
    {
        _revision = revision;
        _course = course;
    }

    #endregion
}