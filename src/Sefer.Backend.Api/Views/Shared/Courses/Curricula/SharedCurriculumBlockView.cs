// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Shared.Courses.Curricula;

/// <summary>
/// A basic view on a curriculum block
/// </summary>
/// <inheritdoc />
public class SharedCurriculumBlockView : AbstractView<CurriculumBlock>
{
    #region Properties

    /// <summary>
    /// The year in which this block is located (when smaller than 1 no years are assumed)
    /// </summary>
    public short? Year => Model.Year;

    /// <summary>
    /// Gets / Sets the sequence number for this CurriculumBlock. Does not have to be unique.
    /// But it will be used for sorting the blocks within a curriculum
    /// </summary>
    public int SequenceId => Model.SequenceId;

    /// <summary>
    /// Gets / sets the name of the CurriculumBlock
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    public string Description => Model.Description;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public SharedCurriculumBlockView(CurriculumBlock model) : base(model) { }

    #endregion
}
