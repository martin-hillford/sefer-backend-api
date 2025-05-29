using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Views.Shared.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Admin.Course.Curricula;

/// <summary>
/// A view on a curriculum block which include the available and selected courses
/// </summary>
/// <inheritdoc />
public class CurriculumBlockView : SharedCurriculumBlockView
{
    #region Properties

    /// <summary>
    /// The list of courses that are selected from this block
    /// </summary>
    public List<Data.JsonViews.CourseView> Courses { get; init; }

    /// <summary>
    /// The list of courses that are available for this block
    /// </summary>
    public List<Data.JsonViews.CourseView> AvailableCourses { get; init; }

    /// <summary>
    /// The id of the curriculum this block belongs to
    /// </summary>
    public int CurriculumId { get; init; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="courses">A list of courses that are selected in this block (should be in the right sequence)</param>\
    /// <param name="availableCourses">a list of courses that are available for this block</param>
    /// <param name="curriculumId">The id of the curriculum this block belongs to</param>
    /// <inheritdoc />
    public CurriculumBlockView(CurriculumBlock model, int curriculumId, List<Data.Models.Courses.Course> courses, List<Data.Models.Courses.Course> availableCourses) : base(model)
    {
        CurriculumId = curriculumId;

        Courses = new List<Data.JsonViews.CourseView>();
        courses?.ForEach(c => Courses.Add(new Data.JsonViews.CourseView(c)));

        AvailableCourses = new List<Data.JsonViews.CourseView>();
        availableCourses?.ForEach(c => AvailableCourses.Add(new Data.JsonViews.CourseView(c)));
    }

    #endregion
}