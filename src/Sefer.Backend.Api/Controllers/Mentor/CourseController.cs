using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class CourseController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    /// <summary>
    /// Gets all the courses the mentor is available for
    /// </summary>
    /// <response code="403">The current user is not a mentor</response>
    [HttpGet("/mentor/courses")]
    [ProducesResponseType(typeof(CourseSummaryView), 200)]
    public async Task<ActionResult<CourseSummaryView>> GetMentorCourses()
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Get the courses
        var revisions = await Send(new GetPublishedCoursesOfMentorRequest(mentor.Id));
        var studentCounts = await Send(new GetStudentsCountPerCourseRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());

        // Create and return the views
        var view = revisions.Select(r => new CourseSummaryView(r.Course, readingTime[r.CourseId], _fileStorageService, studentCounts[r.CourseId]));
        return Json(view);
    }

    /// <summary>
    /// Gets the details of a course for a mentor.
    /// </summary>
    /// <param name="courseId">The id of the course</param>
    /// <response code="403">The current user is not a mentor of the given course</response>
    [HttpGet("/mentor/courses/{courseId:int}")]
    [ProducesResponseType(typeof(ExtendedCourseView), 200)]
    public async Task<ActionResult<ExtendedCourseView>> GetCourseById(int courseId)
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Check if the mentor has access to the course
        var courses = await Send(new GetMentorCoursesRequest(mentor.Id));
        if (courses.All(c => c.Id != courseId)) return Forbid();

        // return the course result
        var course = await Send(new GetPublishedCourseByIdRequest(courseId));
        if (course == null) return NotFound();

        var view = await GetCourseView(course.PublishedCourseRevision);
        return Json(view);
    }

    /// <summary>
    /// Gets the details of a lesson (of a course) for a mentor.
    /// </summary>
    /// <param name="lessonId">The id of the lesson</param>
    /// <response code="403">The current user is not a mentor of the course of the given lesson</response>
    /// <response code="404">A lesson with the given id could not be found</response>
    [HttpGet("/mentor/lessons/{lessonId:int}")]
    [ProducesResponseType(typeof(MentorLessonView), 200)]
    public async Task<IActionResult> GetLessonPreview(int lessonId)
    {
        // Check the mentor
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // Find the lesson
        var lesson = await Send(new GetLessonIncludeReferencesRequest(lessonId));
        if (lesson == null) return NotFound();

        // Check if the mentor has access to the course
        var courses = await Send(new GetMentorCoursesRequest(mentor.Id));
        if (courses.All(c => c.Id != lesson.CourseRevision.CourseId)) return Forbid();

        // Return the lesson to the mentor
        var view = new MentorLessonView(lesson, _fileStorageService);
        return Json(view);
    }

    private async Task<ExtendedCourseView> GetCourseView(CourseRevision revision, int? count = null)
    {
        var courseId = revision.CourseId;
        var lessons = revision.Lessons ?? await Send(new GetLessonsByCourseRevisionRequest(revision.Id));
        var rating = await Send(new GetCourseRatingRequest(courseId));
        var studentCount = count ?? await Send(new GetCourseStudentsCountRequest(courseId));
        var readingTime = await Send(new GetCourseReadingTimeRequest(courseId));
        return new ExtendedCourseView(revision, readingTime[courseId], lessons, rating, _fileStorageService, studentCount);
    }
}