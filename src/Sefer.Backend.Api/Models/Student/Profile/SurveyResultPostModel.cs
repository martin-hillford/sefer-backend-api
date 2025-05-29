// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// This method contains the result of the basic survey presented to the user after a course complete
/// </summary>
public class SurveyResultPostModel
{
    /// <summary>
    /// The rating given to the course (between 0 and 5)
    /// </summary>
    [Required, Range(0, 5)]
    public byte CourseRating { get; set; }

    /// <summary>
    /// The rating given to the mentor (between 0 and 5)
    /// </summary>
    [Required, Range(0, 5)]
    public byte MentorRating { get; set; }

    /// <summary>
    /// The enrollment of which survey this about
    /// </summary>
    [Required]
    public int EnrollmentId { get; set; }

    /// <summary>
    /// The testimony of the student about the course
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Holds if the student gives permission to use his testimony on social media
    /// </summary>
    [Required]
    public bool SocialPermissions { get; set; }
}