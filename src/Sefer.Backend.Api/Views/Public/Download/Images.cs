// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global

using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

public class Images(Data.Models.Courses.Course course)
{
    public string Header { get; set; } = course.HeaderImage;

    public string Thumbnail { get; set; } = course.ThumbnailImage;

    public string Large { get; set; } = course.LargeImage;

    internal async Task IncludeMedia(DownloadRequest request, Course course)
    {
        // First, add a resource for the images of the course
        var header = await ContentSupport.CreateResource(request, Header);
        if (header != null) Header = header.GetResourceUrl();
        if (header != null) course.Resources.Add(header);
        
        var large = await ContentSupport.CreateResource(request, Large);
        if (large != null) Large = large.GetResourceUrl();
        if (large != null) course.Resources.Add(large);
        
        var thumbnail = await ContentSupport.CreateResource(request, Thumbnail);
        if (thumbnail != null) Thumbnail = thumbnail.GetResourceUrl();
        if (thumbnail != null) course.Resources.Add(thumbnail);
    }
}