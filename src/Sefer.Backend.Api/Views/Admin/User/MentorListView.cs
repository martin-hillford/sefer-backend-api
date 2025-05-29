// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.User;

/// <summary>
/// A base view for a mentor for usage in lists for the admin
/// </summary>
/// <inheritdoc />
public class MentorListView : UserListView
{
    #region Properties

    /// <summary>
    /// Gets if the rating of the mentor
    /// </summary>
    public readonly int Rating;

    /// <summary>
    /// Gets the number of rating of the mentor
    /// </summary>
    public readonly int RatingCount;

    /// <summary>
    /// Gets the number of active students for this mentor
    /// </summary>
    public readonly int ActiveStudents;

    /// <summary>
    /// Gets the preferred number of students for this mentor
    /// </summary>
    public readonly int PreferredStudents;

    /// <summary>
    /// Gets the maximum number of students for this mentor
    /// </summary>
    public readonly int MaximumStudents;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="activeStudents">The number of active students</param>
    /// <param name="ratingCount">The rating count for the mentor</param>
    /// <param name="rating">The rating of the mentor (will be between 0 and 10) (inclusive)</param>
    /// <inheritdoc />
    public MentorListView(Data.Models.Users.User model, int activeStudents, int ratingCount, int rating) : base(model)
    {
        // Deal with the mentor settings
        if (model.MentorSettings == null)
        {
            PreferredStudents = 0;
            MaximumStudents = 0;
        }
        else
        {
            PreferredStudents = model.MentorSettings.PreferredStudents;
            MaximumStudents = model.MentorSettings.MaximumStudents;
        }

        // Set the ratings
        RatingCount = ratingCount;
        Rating = rating;

        // Deal with the active students
        ActiveStudents = activeStudents;
    }

    #endregion
}
