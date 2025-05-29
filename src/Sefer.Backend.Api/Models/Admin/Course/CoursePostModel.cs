using Sefer.Backend.Api.Data.Models.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// A course object for posting and putting
/// </summary>
public class CoursePostModel
{
    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    [Required]
    [MaxLength(255)]
    [MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the author of the course
    /// </summary>
    [MaxLength(255)]
    public string Author { get; set; }

    /// <summary>
    /// A link to the webshop where the course can be ordered on paper
    /// </summary>
    public string WebshopLink { get; set; }

    /// <summary>
    /// A link to the video introduction of the course
    /// </summary>
    public string IntroductionLink { get; set; }

    /// <summary>
    /// A notice about who has the copyright of this course
    /// </summary>
    public string Copyright { get; set; }

    /// <summary>
    /// A full https link to a logo to be shown with the copyright notice
    /// </summary>
    public string CopyrightLogo { get; set; }

    /// <summary>
    /// Gets the permalink for the course. This should be a unique entry.
    /// </summary>
    [RegularExpression(PermalinkFormatAttribute.Format)]
    public string Permalink { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the course
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level { get; set; }

    /// <summary>
    /// When this is set to true, the Course will be displayed on the homepage.
    /// Of course only when the Course has a published Revision!
    /// </summary>
    [Required]
    public bool ShowOnHomepage { get; set; }

    /// <summary>
    /// Indicates if this course is a video based or a text based course
    /// </summary>
    [Required]
    public bool IsVideoCourse { get; set; }

    /// <summary>
    /// When this is set to true, the course will be a private course. And will not show up somewhere
    /// That is User are able to take the course using a special url for the course in conjunction with a mentor
    /// </summary>
    [Required]
    public bool Private { get; set; }

    /// <summary>
    /// The maximum number of lesson to submit per day for this course per student
    /// </summary>
    public int? MaxLessonSubmissionsPerDay { get; set; }

    /// <summary>
    /// A citation to show about this course
    /// </summary>
    public string Citation { get; set; }

    /// <summary>
    /// Creates a nice database model from the post
    /// </summary>
    /// <returns></returns>
    public Data.Models.Courses.Course ToModel()
    {
        return new Data.Models.Courses.Course
        {
            Name = Name,
            Permalink = Permalink,
            Description = Description,
            IsVideoCourse = IsVideoCourse,
            Level = Level,
            Private = Private,
            Author = Author,
            ShowOnHomepage = ShowOnHomepage,
            MaxLessonSubmissionsPerDay = MaxLessonSubmissionsPerDay,
            WebshopLink = WebshopLink,
            IntroductionLink = IntroductionLink,
            Copyright = Copyright,
            CopyrightLogo = CopyrightLogo,
            Citation = Citation
        };
    }
}
