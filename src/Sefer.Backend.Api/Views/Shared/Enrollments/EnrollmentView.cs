// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global MemberCanBeProtected.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Enrollments;

/// <summary>
/// An enrollment holds information on the enrollment of a user in a course
/// </summary>
/// <inheritdoc />
public class EnrollmentView : AbstractView<Enrollment>
{
    #region Properties

    /// <summary>
    /// The id of the accountability partner that is supporting the student enrolled to the course.
    /// </summary>
    public int? AccountabilityPartnerId => Model.AccountabilityPartnerId;

    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    public DateTime? ClosureDate => Model.ClosureDate;

    /// <summary>
    /// The date the object was created
    /// </summary>
    public DateTime EnrollmentDate => Model.CreationDate;

    /// <summary>
    /// Holds if the User has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    public bool IsCourseCompleted => Model.IsCourseCompleted;

    /// <summary>
    /// Gets if the enrollment is active
    /// </summary>
    public bool IsActive => Model.IsActive;

    /// <summary>
    /// Returns if this enrollment is a self study enrollment
    /// </summary>
    public bool IsSelfStudy => Model.IsSelfStudy;

    /// <summary>
    /// Holds if the enrollment is imported from the old system
    /// </summary>
    /// <value></value>
    public bool Imported => Model.Imported;

    /// <summary>
    /// Holds if the enrollment was on paper
    /// </summary>
    public bool OnPaper => Model.OnPaper;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="enrollment">The enrollment of the view</param>
    /// <inheritdoc />
    protected EnrollmentView(Enrollment enrollment) : base(enrollment) { }

    #endregion
}