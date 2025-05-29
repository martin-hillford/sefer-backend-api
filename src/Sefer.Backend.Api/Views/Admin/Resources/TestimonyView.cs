// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Resources;

/// <summary>
/// Provides a view on a testimony, can include the course or student
/// </summary>
public class TestimonyView : AbstractView<Testimony>
{
    #region Properties

    /// <summary>
    /// The (optional) id of the course of which this testimony is about
    /// </summary>
    public int? CourseId => Model.CourseId;

    /// <summary>
    /// The optional id of the student that the testimony.
    /// </summary>
    /// <remarks>Please note, this should not be public, but can be seen by admins!</remarks>
    public int? StudentId => Model.StudentId;

    /// <summary>
    /// The testimony itself
    /// </summary>
    public string Content => Model.Content;

    /// <summary>
    /// The (optional) name of the person who made this testimony
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// True when the testimony was anonymous
    /// </summary>
    public bool IsAnonymous => Model.IsAnonymous;

    /// <summary>
    /// The date the object was created
    /// </summary>
    public DateTime CreationDate => Model.CreationDate;

    /// <summary>
    /// The date the object was modified for the last time
    /// </summary>
    public DateTime? ModificationDate => Model.ModificationDate;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public TestimonyView(Testimony model) : base(model) { }

    #endregion
}