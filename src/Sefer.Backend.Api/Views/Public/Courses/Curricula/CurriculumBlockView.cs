// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Views.Shared.Courses;
using Sefer.Backend.Api.Views.Shared.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Public.Courses.Curricula;

/// <summary>
/// A view for a public curriculum with courses
/// </summary>
public sealed class CurriculumBlockView : SharedCurriculumBlockView
{
    /// <summary>
    /// Holds a list of courses
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<CourseDisplayView> Courses { get; set; }

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="fileStorageService">the service for file storage (information)</param>
    /// <inheritdoc />
    public CurriculumBlockView(CurriculumBlock model, IFileStorageService fileStorageService) : base(model)
    {
        if (model.Courses != null)
        {
            Courses = model.Courses.Select(c => new CourseDisplayView(c, fileStorageService)).ToList();
        }
    }
}