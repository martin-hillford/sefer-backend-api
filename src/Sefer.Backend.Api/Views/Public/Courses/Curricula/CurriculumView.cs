// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.Models.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Public.Courses.Curricula;

/// <summary>
/// A view for a public curriculum with courses
/// </summary>
public sealed class CurriculumView : Shared.Courses.Curricula.CurriculumView
{
    /// <summary>
    /// All the blocks in the curriculum if the curriculum is not divided into years
    /// </summary>
    public List<CurriculumBlockView> Blocks { get; set; }

    /// <summary>
    /// Holds the number of year in this curriculum
    /// </summary>
    public bool HasYears { get; set; }

    /// <summary>
    /// When the curriculum is divided into years the blocks are split up
    /// </summary>
    public List<List<CurriculumBlockView>> Years { get; set; }

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="fileStorageService">the service for file storage (information)</param>
    /// <inheritdoc />
    public CurriculumView(Curriculum model, IFileStorageService fileStorageService) : base(model)
    {
        var revision = model.PublishedCurriculumRevision;
        if (revision?.Blocks == null) return;

        // Determine if the curriculum must be split up in blocks or years
        HasYears = revision.Blocks.Any(b => b.Year > 0);
        var blocks = revision.Blocks.Select(b => new CurriculumBlockView(b, fileStorageService));
        if (HasYears == false) Blocks = blocks.ToList();
        else Years = blocks.GroupBy(b => b.Year).OrderBy(b => b.Key).Select(g => g.ToList()).ToList();
    }
}