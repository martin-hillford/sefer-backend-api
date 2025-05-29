namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// CourseEditView is a view specially for editing courses.
/// </summary>
public class CourseEditView : Data.JsonViews.CourseView
{
    #region Private Properties

    /// <summary>
    /// A service that is storing files
    /// </summary>
    protected readonly IFileStorageService FileStorageService;

    /// <summary>
    /// The underlying model
    /// </summary>
    protected readonly Data.Models.Courses.Course Model;

    #endregion

    #region Properties

    /// <summary>
    /// The maximum number of lesson to submit per day for this course per student
    /// </summary>
    public int? MaxLessonSubmissionsPerDay => Model.MaxLessonSubmissionsPerDay;

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public new string HeaderImage => FileStorageService.GetUrl(Model.HeaderImage);

    /// <summary>
    /// Gets the Thumbnail image for the course. Setting of this image will be via a file upload.
    /// </summary>
    public new string ThumbnailImage => FileStorageService.GetUrl(Model.ThumbnailImage);

    /// <summary>
    /// Gets the Large image for the course. Setting of this image will be via a file upload.
    /// </summary>
    public new string LargeImage => FileStorageService.GetUrl(Model.LargeImage);

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="fileStorageService">A service that is storing files</param>
    public CourseEditView(Data.Models.Courses.Course model, IFileStorageService fileStorageService) : base(model)
    {
        FileStorageService = fileStorageService;
        Model = model;
    }

    #endregion
}