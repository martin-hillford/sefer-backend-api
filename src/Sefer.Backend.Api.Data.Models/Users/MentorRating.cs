namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// This model contains a rating provided by a user on completing a course
/// </summary>
/// <inheritdoc />
public class MentorRating : Entity
{
    #region Properties

    /// <summary>
    /// The date the object was created
    /// </summary>
    [InsertOnly]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The id of the mentor that was rated
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The rating performed
    /// </summary>
    [Range(0, 10)]
    public byte Rating { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The mentor of the rating
    /// </summary>
    [ForeignKey("MentorId")]
    public User Mentor { get; set; }

    #endregion
}