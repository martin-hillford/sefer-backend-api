// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A base view for a course
/// </summary>
public class CourseView
{
    #region Properties

    /// <summary>
    /// The id of the model
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the author of the course
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// A link to the webshop where the course can be ordered on paper
    /// </summary>
    public string WebshopLink { get; set; }

    /// <summary>
    /// A link to YouTube or other movie that introduces the course
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
    /// Holds if this course has web shop and this is available on paper
    /// </summary>
    /// <returns></returns>
    public bool HasWebshopLink => string.IsNullOrEmpty(WebshopLink) == false && WebshopLink.StartsWith("http");

    /// <summary>
    /// Holds if this course has introduction link
    /// </summary>
    /// <returns></returns>
    public bool HasIntroductionLink => string.IsNullOrEmpty(IntroductionLink) == false && IntroductionLink.StartsWith("http");

    /// <summary>
    /// Gets the state of the course
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stages? Stage { get; set; }

    /// <summary>
    /// Gets the permalink for the course. This should be a unique entry.
    /// </summary>
    public string Permalink { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the course
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level { get; set; }

    /// <summary>
    /// When this is set to true, the Course will be displayed on the homepage.
    /// Of course only when the Course has a published Revision!
    /// </summary>
    public bool ShowOnHomepage { get; set; }

    /// <summary>
    /// Indicates if this course is a video based or a text based course
    /// </summary>
    public bool IsVideoCourse { get; set; }

    /// <summary>
    /// When this is set to true, the course will be a private course. And will not show up somewhere
    /// That is User are able to take the course using a special url for the course in conjunction with a mentor
    /// </summary>
    public bool Private { get; set; }

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public string LargeImage { get; set; }

    /// <summary>
    /// Gets the Thumbnail image for the course. Setting of this image will be via a file upload.
    /// </summary>
    public string ThumbnailImage { get; set; }

    /// <summary>
    /// A nice citation of the course to use somewhere on the website
    /// </summary>
    public string Citation { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="course">The model of the view</param>
    public CourseView(Course course)
    {
        Id = course.Id;
        Name = course.Name;
        Author = course.Author;
        WebshopLink = course.WebshopLink;
        Copyright = course.Copyright;
        Stage = course.OverallStage;
        Permalink = course.Permalink;
        Description = course.Description;
        Level = course.Level;
        ShowOnHomepage = course.ShowOnHomepage;
        IsVideoCourse = course.IsVideoCourse;
        Private = course.Private;
        HeaderImage = course.HeaderImage;
        LargeImage = course.LargeImage;
        ThumbnailImage = course.ThumbnailImage;
        IntroductionLink = course.IntroductionLink;
        Citation = course.Citation;
        CopyrightLogo = course.CopyrightLogo;
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    public CourseView() { }

    #endregion
}
