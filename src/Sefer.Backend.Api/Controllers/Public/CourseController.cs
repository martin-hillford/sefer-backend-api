using Sefer.Backend.Api.Views.Public.Lessons;

namespace Sefer.Backend.Api.Controllers.Public;

public class CourseController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    [HttpGet("/public/courses/")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(List<CourseSummaryView>), 200)]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await Send(new GetPublishedCoursesRequest());
        var studentCounts = await Send(new GetStudentsCountPerCourseRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());
        var view = courses.Select(c => new CourseSummaryView(c, readingTime[c.Id], _fileStorageService, studentCounts[c.Id]));
        return Json(view);
    }

    [HttpGet("/public/courses/{courseId:int}/preview")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(LessonView), 200)]
    public async Task<IActionResult> GetCoursePreview(int courseId)
    {
        var course = await Send(new GetPublishedCourseByIdRequest(courseId));
        if (course == null) return NotFound();

        var lessons = await Send(new GetLessonsRequest(course.PublishedCourseRevision));
        var lesson = await Send(new GetLessonIncludeReferencesRequest(lessons?.FirstOrDefault()?.Id));

        var view = new LessonView(lesson, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/public/courses/permalink/{permalink}/preview")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(LessonView), 200)]
    public async Task<IActionResult> GetCoursePreviewByPermalink(string permalink)
    {
        var course = await Send(new GetPublishedCourseByPermalinkRequest(permalink));
        if (course == null) return NotFound();
        return await GetCoursePreview(course.Id);
    }

    [HttpGet("/public/courses/{id:int}")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(CourseListView), 200)]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var course = await Send(new GetPublishedCourseByIdRequest(id));
        return await GetCourseView(course);
    }

    [HttpGet("/public/courses/permalink/{permalink}")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(CourseListView), 200)]
    public async Task<IActionResult> GetCourseByPermalink(string permalink)
    {
        var course = await Send(new GetPublishedCourseByPermalinkRequest(permalink));
        return await GetCourseView(course);
    }

    private async Task<IActionResult> GetCourseView(Course course)
    {
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