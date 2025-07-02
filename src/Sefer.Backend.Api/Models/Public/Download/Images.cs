namespace Sefer.Backend.Api.Models.Public.Download;

public class Images(Data.Models.Courses.Course course)
{
    public string Header => course.HeaderImage;
    
    public string Thumbnail => course.ThumbnailImage;
    
    public string Large => course.LargeImage;
}