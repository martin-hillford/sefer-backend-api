using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Views.Shared.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Admin.Course.Curricula;

/// <summary>
/// A view on the curriculum with an editing revision and a published revision
/// </summary>
/// <inheritdoc />
public class CurriculumRevisionsView : CurriculumView
{
    /// <summary>
    /// The current published revision
    /// </summary>
    public readonly CurriculumRevisionView PublishedRevision;

    /// <summary>
    /// The editing revision of the course
    /// </summary>
    public readonly CurrentRevisionView EditingRevision;

    /// <summary>
    /// Creates a CurriculumRevisionsView, a view of a curriculum with the editing revision (and lessons) and the published revision
    /// </summary>
    /// <param name="curriculum">The curriculum</param>
    /// <inheritdoc />
    public CurriculumRevisionsView(Curriculum curriculum) : base(curriculum)
    {
        if (curriculum.PublishedCurriculumRevision != null) PublishedRevision = new CurriculumRevisionView(curriculum.PublishedCurriculumRevision);
        if (curriculum.EditingCurriculumRevision != null) EditingRevision = new CurrentRevisionView(curriculum.EditingCurriculumRevision);
    }
}