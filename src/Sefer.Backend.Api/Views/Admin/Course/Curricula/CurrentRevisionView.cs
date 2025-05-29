// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.Models.Courses.Curricula;
using Sefer.Backend.Api.Views.Shared.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Admin.Course.Curricula;

/// <summary>
/// Creates a view on the current editing revision of the curriculum
/// </summary>
/// <inheritdoc />
public class CurrentRevisionView : CurriculumRevisionView
{
    /// <summary>
    /// A list of all the blocks in this revision
    /// </summary>
    public readonly ReadOnlyCollection<SharedCurriculumBlockView> Blocks;

    /// <summary>
    /// Creates a view on the revision with blocks
    /// </summary>
    /// <param name="revision">The revision with the blocks</param>
    /// <inheritdoc />
    public CurrentRevisionView(CurriculumRevision revision) : base(revision)
    {
        var blocks = new List<SharedCurriculumBlockView>();
        if (revision?.Blocks != null)
        {
            blocks = revision.Blocks
                .Select(b => new SharedCurriculumBlockView(b))
                .ToList();
        }

        Blocks = blocks.AsReadOnly();
    }
}