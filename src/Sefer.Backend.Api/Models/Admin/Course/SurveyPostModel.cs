// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// Is the post object for survey information
/// </summary>
public class SurveyPostModel
{
    /// <summary>
    ///  When set to true, the user will be offered a survey after completing the course. Default is true.
    /// </summary>
    [Required]
    public bool EnableSurvey { get; set; }

    /// <summary>
    /// When set to true, the user will be asked to rate the course. Default is true.
    /// </summary>
    [Required]
    public bool EnableCourseRating { get; set; }

    /// <summary>
    /// When set to true, the user will be asked to rate the mentor. Default is true.
    /// </summary>
    [Required]
    public bool EnableMentorRating { get; set; }

    /// <summary>
    /// When set to true, the user will be given the option to give a testimony (free text). Default is true.
    /// </summary>
    [Required]
    public bool EnableTestimonial { get; set; }

    /// <summary>
    ///  When set to true, the user will be asked if he allows to post snippets from his testimony online. Default is true.
    /// </summary>
    [Required]
    public bool EnableSocialPermissions { get; set; }
}
