using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Courses;

/// <summary>
/// A CourseDisplayView is a CourseView with replace permalink and images so it's ready for display
/// </summary>
/// <inheritdoc />
public class CourseDisplayView : CourseView
{
    /// <summary>
    /// A service that is storing files
    /// </summary>
    protected readonly IFileStorageService FileStorageService;

    /// <summary>
    /// The underlying model
    /// </summary>
    protected readonly Course Model;

    /// <summary>
    /// Gets the permalink for the course. This should be a unique entry.
    /// </summary>
    public new readonly string Permalink;

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public new string HeaderImage => FileStorageService.GetUrl(Model.HeaderImage);

    /// <summary>
    /// Gets the LargeImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public new string LargeImage => FileStorageService.GetUrl(Model.LargeImage);

    /// <summary>
    /// Gets the Thumbnail image for the course. Setting of this image will be via a file upload.
    /// </summary>
    public new string ThumbnailImage => FileStorageService.GetUrl(Model.ThumbnailImage);

    /// <summary>
    /// Creates a new View
    /// </summary>
    public CourseDisplayView(Course model, IFileStorageService fileStorageService) : base(model)
    {
        Model = model;
        FileStorageService = fileStorageService;
        if (!string.IsNullOrEmpty(model.Permalink)) Permalink = "/course/" + Model.Permalink;
        else Permalink = "/public/course/" + model.Id;
    }
}
