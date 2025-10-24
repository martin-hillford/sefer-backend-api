namespace Sefer.Backend.Api.Controllers.App;

/// <summary>
/// This controller deals with course information requests from the app
/// </summary>
/// <param name="serviceProvider"></param>
public class CourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();
    
    /// <summary>
    /// This method delivers the 
    /// </summary>
    /// <param name="courseId"></param>
    /// <returns></returns>
    [HttpGet("/app/courses/{courseId:int}")]
    [ResponseCache(Duration = 1209600)]
    public async Task<IActionResult> GetCourseOverView(int? courseId)
    {
        var course = await Send(new GetPublishedCourseByIdRequest(courseId));
        if (course == null) return NotFound();
        
        var lessons = await Send(new GetLessonsRequest(course.PublishedCourseRevision));
        var rating = await Send(new GetCourseRatingRequest(course.Id));
        var studentCount = await Send(new GetCourseStudentsCountRequest(course.Id));
        var readingTime = await Send(new GetCourseReadingTimeRequest(course.Id));

        course.PublishedCourseRevision.Course ??= course;

        var view = new ExtendedCourseView(course.PublishedCourseRevision, readingTime[course.Id], lessons, rating, _fileStorageService, studentCount);
        return Json(view);
    }
}




